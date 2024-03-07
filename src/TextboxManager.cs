using System;
using System.Linq;
using Godot;

public partial class TextboxManager : Control
{
    [Signal] public delegate void DialogueStartedEventHandler();
    [Signal] public delegate void DialogueFinishedEventHandler();

    [Signal] public delegate void DialogueClosedEventHandler();

    [Export] private RichTextLabel textbox;
    [Export] private HBoxContainer choiceButtonContainerNode;
    [Export] private PackedScene choiceButtonScene;
    private DialogueNode currNode;
    
    public const double TextboxTimeout = 0.1;
    private double textboxTimer;
    private bool FinishedTextboxCooldown => textboxTimer > TextboxTimeout;
    private void StartTextboxCooldown() => textboxTimer = 0;
    private bool IsTextDisplayed => textbox.VisibleCharacters >= textbox.GetTotalCharacterCount();
    private bool CurrentlyDisplayingChoices => choiceButtonContainerNode.SizeFlagsVertical.HasFlag(SizeFlags.ExpandFill);
    private Callable displayChoicesHook;
    private double timePerCharacter = 0.01;
    private double textboxInterpolation;
    private int prevVisibleCharacters;

    public override void _Ready()
    {
        textboxTimer = 0;
        Visible = false;
        DialogueStarted += () => GetTree().Paused = true;
        DialogueClosed += () => GetTree().Paused = false;
    }

    public override void _Process(double delta)
    {
        textboxTimer += delta;
        textboxInterpolation += delta;
        if (!IsTextDisplayed)
            ScrollText();
    }

    public void ScrollText()
    {
        int visibleChars = Tween.InterpolateValue(0, textbox.GetTotalCharacterCount(),
            textboxInterpolation, timePerCharacter * textbox.GetTotalCharacterCount(),
        Tween.TransitionType.Linear, Tween.EaseType.In).AsInt32();
        if (textbox.VisibleCharacters >= textbox.GetTotalCharacterCount())
            textbox.VisibleCharacters = textbox.GetTotalCharacterCount();
        else
            textbox.VisibleCharacters = visibleChars;
        if (prevVisibleCharacters < textbox.GetTotalCharacterCount() &&
            visibleChars >= textbox.GetTotalCharacterCount())
            EmitSignal(nameof(DialogueFinished));
        prevVisibleCharacters = visibleChars;
    }
    
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_accept") && textbox.Visible)
        {
            if (!FinishedTextboxCooldown) return;
            
            if (textbox.VisibleCharacters == textbox.GetTotalCharacterCount() && 
                !CurrentlyDisplayingChoices)
                NextTextBox();
            else
            {
                EmitSignal(nameof(DialogueFinished));
                textbox.VisibleCharacters = textbox.GetTotalCharacterCount();
            }
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
        Visible = true;
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
        Visible = false;
        EmitSignal(nameof(DialogueClosed));
    }

    public void TextBoxWithTextNode()
    {
        textbox.Text = currNode.Texts[0];
        PlayTextbox();
        currNode = currNode.Next.First();
    }

    public void TextBoxWithChoiceNode()
    {
        displayChoicesHook = new Callable(this, nameof(DisplayChoices));
        Connect(nameof(DialogueFinished), displayChoicesHook);
        textbox.Text = currNode.Texts[0];
        PlayTextbox();
    }

    public void DisplayChoices()
    {
        // add choice buttons with the path
        // add choice button container:
        choiceButtonContainerNode.SizeFlagsVertical = SizeFlags.ExpandFill;
        
        int numChoices = currNode.Texts.Length - 1;
        for (int i = 0; i < numChoices; i++)
        {
            // Button choiceNode = ResourceLoader.Load<PackedScene>(ChoiceButtonPath).Instantiate<Button>();
            Button choiceNode = choiceButtonScene.Instantiate<Button>();
            choiceButtonContainerNode.AddChild(choiceNode);
            int choiceNum = i;
            choiceNode.Pressed += () => ChoiceButtonPressed(choiceNum);
        }
        // Disconnect the hook after being called once
        Disconnect(nameof(DialogueFinished), displayChoicesHook);
    }
    
    public void RemoveChoices()
    {
        choiceButtonContainerNode.SizeFlagsVertical = SizeFlags.ShrinkEnd;
        foreach (var child in choiceButtonContainerNode.GetChildren())
            child.QueueFree();
    }
    
    
    public void ChoiceButtonPressed(int choiceNum)
    {
        if (choiceNum >= currNode.Next.Length || choiceNum < 0)
            GD.PrintErr($"Choice button pressed out of range: choiceNum = {choiceNum}");
        currNode = currNode.Next[choiceNum];
        RemoveChoices();
        NextTextBox();
    }
    
    public void BranchNode()
    {
    }

    private void PlayTextbox()
    {
        EmitSignal(nameof(DialogueStarted));
        textboxInterpolation = 0;
        textbox.VisibleCharacters = 0;
    }
}
