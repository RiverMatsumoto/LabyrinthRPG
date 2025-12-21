using System.Linq;

public class ChoiceNode : DialogueNode
{
    public string Prompt { get; }

    public ChoiceNode(string prompt, string[] choices, DialogueNode[] nextNodes)
        : base(texts: new string[0], next: nextNodes)
    {
        Prompt = prompt;
        Texts = new[] { prompt }.Concat(choices).ToArray();
    }
}
