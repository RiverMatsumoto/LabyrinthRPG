using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MEC;

public partial class TextboxManager : Control
{
    [Signal]
    public delegate void DialogueStartedEventHandler();
    [Signal]
    public delegate void DialogueFinishedEventHandler();

    private Control textboxNode;
    private RichTextLabel textbox;
    private DialogueNode currNode;
    private bool printAllTextFlag;
    private const string TextboxContainer = "./VBoxContainer";
    private const string ChoiceButtonContainer = "./VBoxContainer/ChoiceButtonContainer";
    private const string ChoiceButtonContainerPath = "res://src/scenes/ChoiceButtonContainer.tscn";
    private const string ChoiceButtonPath = "res://src/scenes/ChoiceButton.tscn";
    private const double TextboxTimeout = 0.1;
    private double textboxTimer;
    private bool CanInteract => textboxTimer > TextboxTimeout;
    private Callable displayChoicesHook;

    public override void _Ready()
    {
        textboxTimer = 0;
        printAllTextFlag = false;
        textboxNode = GetNode<Control>("TextboxScene");
        textbox = textboxNode.GetNode<RichTextLabel>("./VBoxContainer/Textbox");
        textboxNode.Visible = false;
    }

    public override void _Process(double delta)
    {
        textboxTimer += delta;
    }

    IEnumerator<double> ScrollText()
    {
        printAllTextFlag = false;
        textbox.VisibleCharacters = 0;
        while (!printAllTextFlag && textbox.VisibleCharacters < textbox.GetTotalCharacterCount())
        {
            textbox.VisibleCharacters += 1;
            yield return Timing.WaitForOneFrame;
        }
        printAllTextFlag = false;
        textbox.VisibleCharacters = textbox.GetTotalCharacterCount();
        EmitSignal(nameof(DialogueFinished));
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_accept") && textbox.Visible)
        {
            if (!CanInteract) return;
            
            if (textbox.VisibleCharacters == textbox.GetTotalCharacterCount() && 
                !textboxNode.HasNode(ChoiceButtonContainer))
                NextTextBox();
            else 
                printAllTextFlag = true;
            textboxTimer = 0;
        }
        else if (@event.IsActionPressed("DEBUG_1"))
        {
            var textNode = new TextNode("Helloasdjkfa;sdkfjas;ldjkfasjf", null);
            currNode = new StartNode(textNode);
            OpenDialogue();
        }
        else if (@event.IsActionPressed("DEBUG_2"))
        {
            DialogueNode[] nexts = 
            {
                new TextNode("Choice 1 was chosen", null),
                new TextNode("Choice 2 was chosen", null),
                new TextNode("Choice 3 was chosen", null),
                new TextNode("Choice 4 was chosen", null)
            };
            string[] choices = {"Choice 1", "Choice 2", "Choice 3", "Choice 4"};
            var choiceNode = new ChoiceNode("Helloasdjkfa; PROMPT PROMPT PROMPT PROMPT PROMPT", choices, nexts);
            currNode = new StartNode(choiceNode);
            OpenDialogue();
        }
    }

    public void OpenDialogue()
    {
        textboxNode.Visible = true;
        NextTextBox();
    }

    public void NextTextBox()
    {
        if (currNode == null)
        {
            CloseDialogue();
            return;
        }
        
        Type nodeType = currNode.GetType();
        if (nodeType == typeof(StartNode))
        {
            currNode = currNode.Next.First();
            NextTextBox();
        }
        else if (nodeType == typeof(TextNode))
        {
            TextBoxWithTextNode();
        }
        else if (nodeType == typeof(ChoiceNode))
        {
            GD.Print("Choicenode");
            TextBoxWithChoiceNode();
        }
    }

    public void CloseDialogue()
    {
        textbox.Text = "";
        textboxNode.Visible = false;
    }

    public void TextBoxWithTextNode()
    {
        textbox.Text = currNode.Texts[0];
        Timing.RunCoroutine(ScrollText());
        currNode = currNode.Next.First();
    }

    public void TextBoxWithChoiceNode()
    {
        displayChoicesHook = new Callable(this, nameof(DisplayChoices));
        Connect(nameof(DialogueFinished), displayChoicesHook);
        textbox.Text = currNode.Texts[0];
        ScrollText().RunCoroutine();
    }

    public void DisplayChoices()
    {
        // add choice buttons with the path
        // add choice button container:
        Node choiceButtonContainer = ResourceLoader.Load<PackedScene>(ChoiceButtonContainerPath).Instantiate();
        Node textboxContainer = textboxNode.GetNode(TextboxContainer);
        textboxContainer.AddChild(choiceButtonContainer);
        
        int numChoices = currNode.Texts.Length - 1;
        for (int i = 0; i < numChoices; i++)
        {
            Button choiceNode = ResourceLoader.Load<PackedScene>(ChoiceButtonPath).Instantiate<Button>();
            choiceButtonContainer.AddChild(choiceNode);
            int choiceNum = i;
            choiceNode.Pressed += () => ChoiceButtonPressed(choiceNum);
        }
        Disconnect(nameof(DialogueFinished), displayChoicesHook);
    }
    
    public void RemoveChoices()
    {
        Node textboxContainer = textboxNode.GetNode(ChoiceButtonContainer);
        textboxContainer.QueueFree();
    }
    
    
    public void ChoiceButtonPressed(int choiceNum)
    {
        if (choiceNum >= currNode.Next.Length || choiceNum < 0)
            GD.PrintErr($"Choice button pressed out of range {choiceNum}");
        currNode = currNode.Next[choiceNum];
        RemoveChoices();
        NextTextBox();
    }
    
    public void BranchNode()
    {
    }
}
