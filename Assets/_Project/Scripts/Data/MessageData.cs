using UnityEngine;

namespace LastSignal.Data
{
    [CreateAssetMenu(fileName = "MSG_New", menuName = "LastSignal/Message")]
    public class MessageData : ScriptableObject
    {
        [Header("Identity")]
        public string senderId;
        public string senderDisplayName;
        public MessageType messageType;

        [Header("Content")]
        [TextArea(3, 8)]
        public string messageText;

        [Header("Choices")]
        public ChoiceData[] choices;

        [Header("Conditions")]
        public int requiredDay = 1;
        public float triggerChance = 1f;
    }

    [System.Serializable]
    public class ChoiceData
    {
        [TextArea(1, 3)]
        public string choiceText;
        public ChoiceConsequence[] consequences;
    }

    [System.Serializable]
    public class ChoiceConsequence
    {
        public ConsequenceType type;
        public string targetId;
        public int value;
    }

    public enum MessageType { Negotiation, Event, Story, System }

    public enum ConsequenceType
    {
        GainResource,
        LoseResource,
        ChangeReputation,
        TriggerEvent,
        TriggerEnding
    }
}
