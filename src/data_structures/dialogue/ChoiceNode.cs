using System.Linq;

public class ChoiceNode : DialogueNode
{
    
    public ChoiceNode(string prompt, string[] choices, DialogueNode[] next) : base(next)
    {
        Texts = Texts.Append(prompt).ToArray();
        Texts = Texts.Concat(choices).ToArray();
    }

}
