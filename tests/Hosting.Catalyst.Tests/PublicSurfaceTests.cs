using System.Reflection;
using Diagrid.Aspire.Hosting.Catalyst;

namespace Hosting.Catalyst.Tests;

public class PublicSurfaceTests
{
    // The baseline is generated from the net10.0 build and shared by every target framework, so a
    // framework-conditional public API makes the net8.0 or net9.0 run fail naming the member.
    [Fact]
    public void PublicSurfaceIsIdenticalOnEveryTargetFramework()
    {
        var actual = PublicSurface.Enumerate(typeof(CatalystProject).Assembly);
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
