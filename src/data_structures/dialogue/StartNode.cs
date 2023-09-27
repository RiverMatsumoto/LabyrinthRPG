using System.Linq;

public class StartNode : DialogueNode
{
    public StartNode(DialogueNode next) : base()
    {
        Next = Next.Append(next).ToArray();
    }
}
