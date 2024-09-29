
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class SpiritSeparator : Interactable
{
    private bool used = false;
    private int answer;
    private int curr;
    public Lever[] levers;
    public Note[] notes;
    public Sprite normalSprite;
    public Sprite chargeSprite;
    public Sprite activateSprite;
    public GameObject outputDuct;
    private SpriteRenderer spriteRenderer;
    
    protected override void Awake()
    {
        base.Awake();
        CanPlayerInteract = false;
        CanSpiritInteract = false;
        curr = 0;
        answer = Random.Range(1, 1 << levers.Length);
        Debug.Log("Answer: " + answer.ToBinaryString());
        for (int i = 0; i < levers.Length; i++)
        {
            int bit = 1 << i;
            levers[i].CanPlayerInteract = true;
            levers[i].CanSpiritInteract = false;
            levers[i].enable.AddListener(() => 
            {
                curr |= bit;
                if (curr == answer)
                {
                    Activate();
                }
            });
            levers[i].disable.AddListener(() =>
            {
                curr &= ~bit;
                if (curr == answer)
                {
                    Activate();
                }
            });
            Note temp = notes[i];
            int r = Random.Range(i, notes.Length);
            notes[i] = notes[r];
            notes[r] = temp;
            notes[i].title = "Old Research";
            notes[i].message = messages[i][(answer & bit) > 0 ? 1 : 0];
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        normalSprite = spriteRenderer.sprite;
    }

    public override void Interact(User user)
    {
        if (used) return;
        CameraSystem cameraSystem = Camera.main.GetComponent<CameraSystem>();
        cameraSystem.PlaySplitAnimation();
        
        CanPlayerInteract = false;
        used = true;
    }
    
    public void Activate()
    {
        if(used) return;
        foreach (Lever lever in levers)
        {
            lever.CanPlayerInteract = false;
        }
        CanPlayerInteract = true;
        spriteRenderer.sprite = chargeSprite;
    }
    
    private static readonly string[][] messages = new string[6][]
    {
        new string[] 
        {
            "We increased and then reduced the oxygen levels, but to no avail. The spirits remained untouched, indifferent to the atmosphere around them. Even as the air thinned and our own breathing became labored, the machine droned on with no sign of spiritual response. It appears oxygen has no bearing on the ethereal realm — they neither need it nor care for its absence.",
            "Subject 13 showed increased spirit manifestation when oxygen levels were elevated. The chamber felt charged, as though the air itself crackled with energy. The presence of oxygen seems to strengthen the link between the living and the spectral. We plan to push it further - last time, the machine seemed to breathe with us, as if waiting for something to emerge."
        },
        new string[] 
        {
            "The hydrogen flowed through the machine, hissing softly as we adjusted its levels, but no matter how much we increased the pressure, the spirits remained distant. The air was thick with expectation, but nothing stirred. Hydrogen, it seems, is irrelevant to them — neither a key to unlock their presence nor a barrier to hold them back.",
            "Upon introducing hydrogen into the machine, the ethereal resonance intensified. The spirits became agitated, drawn closer to our realm but unable to fully manifest. When we turned the flow off, the tension lifted, yet it left a presence behind — a residual weight in the air, as if something was watching from just beyond the veil. Hydrogen seems to spark something ancient in the process."
        },
        new string[] 
        {
            "We had hoped carbon would anchor the spirits, stabilize them within the machine, but the readings were flat. No reaction, no movement. The spirits drifted in and out of reach with or without the carbon adjustment. It seems we misunderstood its role entirely — carbon does not bind them as it does living forms.",
            "When we increased the carbon levels, the spirits became visible — dark, twisted forms hovering above the machine. But instead of fading, they grew stronger. The carbon agent worked too well. The air chilled, and suddenly the shadows attacked, engulfing the researchers. Their faces twisted in silent screams. We shut the machine down, but it was too late."
        },
        new string[] 
        {
            "As the nitrogen cooled the machine, we watched the gauges intently, waiting for a shift, a sign — anything. But the temperature drop was met with silence. The spirits remained unaffected, their presence unchanged. The cold, though uncomfortable for us, held no sway over them. Nitrogen is a false lead; they exist beyond such physical constraints.",
            "Cold. The spirits respond to cold. When the nitrogen was activated, the temperature dropped rapidly, and we felt them — whispers on the edge of our senses. There was a moment, as the frost gathered on the metal, that the boundary thinned enough for a voice to break through. It spoke in fragments, incomprehensible, yet undeniably there. The nitrogen, it seems, cools the body but warms the soul's escape."
        },
        new string[] 
        {
            "The phosphorus was supposed to amplify their presence, but despite our efforts, the spirits showed no signs of reacting. We dialed the levels higher and higher until the machine strained. It was as though phosphorus was invisible to them — no glow, no flicker, no manifestation. They remain impervious to its supposed amplifying effect.",
            "The glow was faint at first, but as we increased the phosphorus, the spirits grew more visible, their forms flickering in and out of our reality. They seemed drawn to it, though whether they were curious or fearful, we could not tell. One moment, it was just us in the room; the next, there were shadows that did not belong to us. Phosphorus does more than amplify — it invites them closer."
        },
        new string[] 
        {
            "Iron, long believed to ward off spirits, has a curious effect when used in the machine. With the conductor active, the spirits recoiled, retreating from whatever process was taking place. The machine slowed, as though something essential was missing. When we turned it off, they returned, cautiously at first, then more confidently. Iron, it seems, is their cage, and without it, they are free to roam.",
            "Iron, once thought to repel spirits, surprisingly enhances their presence within the machine. As the iron coursed through its circuits, the spirits gathered, drawn to its magnetic pull. The machine hummed with newfound energy, thickening the shadows in the air. It seems iron acts as a conduit, inviting them closer and bridging the gap between worlds."
        }
    };

}