# Logo

`diagrid.txt` is the banner printed once at AppHost boot, in block letters at 51 columns so it fits an
80-column terminal.

It replaced five CATALYST banners in five ASCII fonts, picked at random per run, when Catalyst was
renamed to Diagrid. One banner, no rotation: the art was the only thing that needed five files.

`LogoPicker` finds it by this folder's namespace rather than a hardcoded prefix, so renaming the root
namespace cannot silently empty the lookup.
