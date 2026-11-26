using System.Reflection;
using Diagrid.Aspire.Hosting;

namespace Hosting.Diagrid.Tests;

public class PublicSurfaceTests
{
    // Written when the package multi-targeted, to catch an API behind a framework conditional that only
    // one framework would see. Aspire 17 ships net10.0 and net11.0 only, so the package is single-target
    // and that cannot happen today; the baseline now earns its keep as a review gate, failing on any
    // unintended change to the public surface. It becomes a parity guard again if a framework is re-added.
    [Fact]
    public void PublicSurfaceIsIdenticalOnEveryTargetFramework()
    {
        var actual = PublicSurface.Enumerate(typeof(DiagridProject).Assembly);

        // Deliberately changed the public API? Regenerate the baseline instead of transcribing it out of an
        // xunit sequence diff, which is how a guard like this ends up deleted. From the repo root:
        //   CATALYST_WRITE_PUBLIC_SURFACE=$PWD/tests/Hosting.Diagrid.Tests/public-surface.txt \
        //     dotnet test tests/Hosting.Diagrid.Tests --framework net10.0 --filter PublicSurfaceIsIdenticalOnEveryTargetFramework
        // Regenerate from the highest framework the package targets: a lower one would bake its narrower
        // view in if the package ever multi-targets again. This run writes and does not verify; the next
        // ordinary run is what checks the result.
        var destination = Environment.GetEnvironmentVariable("CATALYST_WRITE_PUBLIC_SURFACE");
        if (!string.IsNullOrWhiteSpace(destination))
        {
            // Join with '\n' rather than WriteAllLines: on Windows that writes CRLF, and .gitattributes
            // stores this file with LF, so the regenerated baseline would differ from the committed one in
            // all 245 lines while saying exactly the same thing.
            File.WriteAllText(destination, string.Join('\n', actual) + '\n');
            return;
        }

        var baselinePath = Path.Combine(AppContext.BaseDirectory, "public-surface.txt");
        var expected = File.ReadAllLines(baselinePath).Where(l => l.Length > 0).OrderBy(l => l, StringComparer.Ordinal);
        Assert.Equal(expected, actual);
    }

    // Renders only the assembly's own exported types and the members each declares, from names alone.
    // Compiler-generated members and inherited framework members are excluded because they vary by
    // target framework without any change to the API the package exposes.
    private static class PublicSurface
    {
        private const BindingFlags Declared =
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

        public static List<string> Enumerate(Assembly assembly)
        {
            var lines = new List<string>();
            foreach (var type in assembly.GetExportedTypes())
            {
                lines.Add($"type {Name(type)}");
                foreach (var member in type.GetMembers(Declared))
                {
                    var line = Describe(type, member);
                    if (line is not null)
                    {
                        lines.Add(line);
                    }
                }
            }

            return lines.Distinct(StringComparer.Ordinal).OrderBy(l => l, StringComparer.Ordinal).ToList();
        }

        private static string? Describe(Type owner, MemberInfo member)
        {
            if (member.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), inherit: false))
            {
                return null;
            }

            switch (member)
            {
                case MethodBase method when method.IsPublic || method.IsFamily || method.IsFamilyOrAssembly:
                    // Accessors are covered by their property or event line.
                    if (method is MethodInfo { IsSpecialName: true } && IsAccessor(method.Name))
                    {
                        return null;
                    }

                    var returns = method is MethodInfo info ? Name(info.ReturnType) : "void";
                    var parameters = string.Join(", ", method.GetParameters().Select(p => Name(p.ParameterType)));
                    var methodName = method is ConstructorInfo ? ".ctor" : method.Name;
                    return $"{Name(owner)}.{methodName}({parameters}) : {returns}";
                case PropertyInfo property when property.GetMethod is { IsPublic: true } or { IsFamily: true } or { IsFamilyOrAssembly: true }
                    || property.SetMethod is { IsPublic: true } or { IsFamily: true } or { IsFamilyOrAssembly: true }:
                    var index = string.Join(", ", property.GetIndexParameters().Select(p => Name(p.ParameterType)));
                    var get = property.GetMethod is { IsPublic: true } or { IsFamily: true } or { IsFamilyOrAssembly: true } ? " get;" : string.Empty;
                    var set = property.SetMethod is { IsPublic: true } or { IsFamily: true } or { IsFamilyOrAssembly: true } ? " set;" : string.Empty;
                    return $"{Name(owner)}.{property.Name}[{index}] : {Name(property.PropertyType)} {{{get}{set} }}";
                case FieldInfo field when field.IsPublic || field.IsFamily || field.IsFamilyOrAssembly:
                    return $"{Name(owner)}.{field.Name} : {Name(field.FieldType)}";
                case EventInfo evt:
                    return $"{Name(owner)}.{evt.Name} : {Name(evt.EventHandlerType!)}";
                default:
                    return null;
            }
        }

        private static bool IsAccessor(string name) =>
            name.StartsWith("get_", StringComparison.Ordinal) || name.StartsWith("set_", StringComparison.Ordinal)
            || name.StartsWith("add_", StringComparison.Ordinal) || name.StartsWith("remove_", StringComparison.Ordinal);

        private static string Name(Type type)
        {
            if (type.IsGenericParameter)
            {
                return type.Name;
            }

            if (type.IsArray)
            {
                return Name(type.GetElementType()!) + "[]";
            }

            if (type.IsByRef)
            {
                return Name(type.GetElementType()!) + "&";
            }

            var name = (type.IsNested ? Name(type.DeclaringType!) + "+" : type.Namespace + ".") + type.Name;
            return type.IsGenericType && !type.IsGenericTypeDefinition
                ? name + "<" + string.Join(", ", type.GetGenericArguments().Select(Name)) + ">"
                : name;
        }
    }
}
