using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEngine;

public class Note : Interactable
{
    public string title;
    public string message;

    protected override void Awake()
    {
        base.Awake();
        CanPlayerInteract = true;
        CanSpiritInteract = true;
    }

    public override void Interact(User user)
    {
        // split message into a string array, basically i want as many sentences as possible in each before 150 chars is reached in one string, sentence ends with period
        string current = "";
        List<string> sentences = new List<string>();
        foreach (string sentence in message.Split('.'))
        {
            string trimmed = sentence.Trim();
            if (trimmed.Length == 0) continue;
            bool empty = current.Length == 0;
            if (empty || current.Length + trimmed.Length + 2 <= 150)
            {
                current += (empty  ? "" : " ") + trimmed + ".";
            }
            else
            {
                sentences.Add(current);
                current = trimmed + ".";
            }
        }
        sentences.Add(current);
        user.dialogueManager.Show(new Dialogue(title, sentences.ToArray()));
    }
}