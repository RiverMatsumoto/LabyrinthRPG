
public class StartNode : DialogueNode
{
    public StartNode(DialogueNode next)
        : base(new string[0], new[] { next })
    {
    }
}
