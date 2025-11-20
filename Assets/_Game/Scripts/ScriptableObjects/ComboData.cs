using System.Collections.Generic;
using UnityEngine;
using NeonSyndicate.Combat;

namespace NeonSyndicate.Data
{
    /// <summary>
    /// Combo verilerini tutan ScriptableObject.
    /// Unity Editor'de Create > Neon Syndicate > Combo Data ile oluşturulur.
    /// </summary>
    [CreateAssetMenu(fileName = "New Combo Data", menuName = "Neon Syndicate/Combo Data")]
    public class ComboData : ScriptableObject
    {
        [Header("Combo Information")]
        [SerializeField] private string comboName = "Basic Combo";
        [TextArea(3, 5)]
        [SerializeField] private string description = "Açıklama buraya...";

        [Header("Combo Steps")]
        [SerializeField] private List<ComboStep> comboSteps = new List<ComboStep>();

        public string ComboName => comboName;
        public string Description => description;
        public List<ComboStep> ComboSteps => comboSteps;
    }
}

