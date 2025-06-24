using Nodify.ViewModels;

namespace Nodify.Models;

public class MenuLibrary
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<NodeViewModel> Nodes { get; set; }
}

