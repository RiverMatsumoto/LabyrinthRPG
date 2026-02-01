using System;
using System.Linq;
using Godot;

public partial class TextboxManager : Control
{
    [Signal] public delegate void DialogueStartedEventHandler();
    [Signal] public delegate void DialogueFinishedEventHandler();
    [Signal] public delegate void DialogueClosedEventHandler();

    [Export] private VBoxContainer textboxContainer;
    [Export] private RichTextLabel textbox;
    [Export] private HBoxContainer choiceButtonContainerNode;
    private DialogueNode currNode;

    [Export] private PackedScene choiceButtonScene;
    private Callable displayChoicesCallable;

    public double TextboxTimeout = 0.1;
    private double textboxTimer;
    private bool FinishedTextboxCooldown => textboxTimer > TextboxTimeout;
    private bool IsTextDisplayed => textbox.VisibleCharacters >= textbox.GetTotalCharacterCount();
    private bool CurrentlyDisplayingChoices => choiceButtonContainerNode.SizeFlagsVertical.HasFlag(SizeFlags.ExpandFill);
    private double timePerCharacter = 0.01;
    private double textboxInterpolation;
    private int prevVisibleCharacters;


    public override void _Ready()
    {
        textboxTimer = 0;
        Visible = false;

        // Flow of one text box: DialogueStarted -> DialogueFinished -> DialogueClosed

        // Connect dialogue finished once and manage flow internally
        displayChoicesCallable = new Callable(this, nameof(OnDialogueFinished));
        Connect(nameof(DialogueFinished), displayChoicesCallable);

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

        textbox.VisibleCharacters = Math.Min(visibleChars, textbox.GetTotalCharacterCount());

        if (prevVisibleCharacters < textbox.GetTotalCharacterCount() &&
            visibleChars >= textbox.GetTotalCharacterCount())
        {
            EmitSignal(SignalName.DialogueFinished);
        }
        prevVisibleCharacters = visibleChars;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_accept") && textbox.Visible)
        {
            if (!FinishedTextboxCooldown) return;

            if (textbox.VisibleCharacters == textbox.GetTotalCharacterCount() &&
                !CurrentlyDisplayingChoices)
                DisplayNextTextBox();
            else
            {
                EmitSignal(SignalName.DialogueFinished);
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
            string[] choices = { "Choice 1", "Choice 2", "Choice 3", "Choice 4" };
            var choiceNode = new ChoiceNode("Prompt Text :)", choices, nexts);
            currNode = new StartNode(choiceNode);
            OpenDialogue();
        }
    }

    public void OpenDialogue()
    {
        Visible = true;
        DisplayNextTextBox();
    }

    public void DisplayNextTextBox()
    {
        // always close the textbox on null to be safe
        if (currNode == null)
        {
            CloseDialogue();
            return;
        }
        DisplayNode(currNode);
    }

    public void CloseDialogue()
    {
        textbox.Text = "";
        Visible = false;
        EmitSignal(SignalName.DialogueClosed);
    }

    public void DisplayNode(DialogueNode node)
    {
        switch (node)
        {
            case StartNode startNode:
                currNode = startNode.Next.FirstOrDefault();
                if (currNode != null)
                    DisplayNode(currNode);
                else
                    CloseDialogue();
                break;

            case TextNode textNode:
                ShowTextNode(textNode);
                break;

            case ChoiceNode choiceNode:
                ShowChoiceNode(choiceNode);
                break;

            case EndNode:
                CloseDialogue();
                break;

            default:
                GD.PrintErr($"Unknown DialogueNode type: {node.GetType()}");
                CloseDialogue();
                break;
        }
    }

    public void ShowTextNode(TextNode node)
    {
        RemoveChoices();
        textbox.Text = node.Texts.First();
        PlayTextbox();
        currNode = currNode.Next.First();
    }

    public void ShowChoiceNode(ChoiceNode node)
    {
        RemoveChoices();
        textbox.Text = node.Prompt;
        PlayTextbox();
        currNode = node;
    }

    private void OnDialogueFinished()
    {
        // Only show choices if current node is ChoiceNode and choices are not already shown
        if (currNode is ChoiceNode && !CurrentlyDisplayingChoices)
        {
            DisplayChoices();
        }
    }

    public void DisplayChoices()
    {
        choiceButtonContainerNode.SizeFlagsVertical = SizeFlags.ExpandFill;

        int numChoices = currNode.Texts.Length - 1; // texts[0] is the prompt
        for (int i = 0; i < numChoices; i++)
        {
            var choiceBtn = choiceButtonScene.Instantiate<Button>();
            choiceBtn.Text = currNode.Texts[i + 1];
            choiceButtonContainerNode.AddChild(choiceBtn);

            int choiceIndex = i;
            choiceBtn.Pressed += () => ChoiceButtonPressed(choiceIndex);
        }
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
        DisplayNextTextBox();
    }

    public void BranchNode()
    {
    }

    private void PlayTextbox()
    {
        EmitSignal(SignalName.DialogueStarted);
        textboxInterpolation = 0;
        textbox.VisibleCharacters = 0;
    }
}
