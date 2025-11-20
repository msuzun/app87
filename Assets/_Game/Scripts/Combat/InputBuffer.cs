using System.Collections.Generic;
using UnityEngine;

namespace NeonSyndicate.Combat
{
    /// <summary>
    /// Input Buffer System - Tuşlara erken basıldığında bile algılar.
    /// "Lag hissi" yok eder, responsive gameplay sağlar.
    /// 
    /// Örnek: Oyuncu saldırı animasyonu bitmeden 0.1 saniye önce tuşa basar,
    /// sistem bunu hafızaya alır ve zamanı gelince uygular.
    /// </summary>
    public class InputBuffer : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Input ne kadar süre buffer'da kalır? (saniye)")]
        [SerializeField] private float bufferTime = 0.2f;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = false;

        private Queue<InputRecord> buffer = new Queue<InputRecord>();

        /// <summary>
        /// Buffer'daki input kaydı.
        /// </summary>
        private struct InputRecord
        {
            public InputType type;
            public float timestamp;
        }

        private void Update()
        {
            // Süresi geçen inputları temizle
            CleanExpiredInputs();
        }

        /// <summary>
        /// Yeni bir input kaydeder (buffer'a ekler).
        /// </summary>
        public void RegisterInput(InputType type)
        {
            if (type == InputType.None) return;

            buffer.Enqueue(new InputRecord 
            { 
                type = type, 
                timestamp = Time.time 
            });

            if (showDebugInfo)
            {
                Debug.Log($"[InputBuffer] Registered: {type} at {Time.time:F2}s");
            }
        }

        /// <summary>
        /// Buffer'daki en eski inputu döner (ama silmez).
        /// Peek işlemi.
        /// </summary>
        public InputType TryGetBufferedInput()
        {
            if (buffer.Count > 0)
            {
                return buffer.Peek().type;
            }
            return InputType.None;
        }

        /// <summary>
        /// Buffer'daki en eski inputu tüketir (siler).
        /// Kullanıldıktan sonra çağrılmalı.
        /// </summary>
        public void ConsumeInput()
        {
            if (buffer.Count > 0)
            {
                InputType consumed = buffer.Dequeue().type;
                
                if (showDebugInfo)
                {
                    Debug.Log($"[InputBuffer] Consumed: {consumed}");
                }
            }
        }

        /// <summary>
        /// Belirli bir input tipini arar ve tüketir.
        /// </summary>
        public bool TryConsumeSpecificInput(InputType type)
        {
            if (buffer.Count > 0 && buffer.Peek().type == type)
            {
                ConsumeInput();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Buffer'ı tamamen temizler.
        /// </summary>
        public void ClearBuffer()
        {
            buffer.Clear();
            
            if (showDebugInfo)
            {
                Debug.Log("[InputBuffer] Buffer cleared");
            }
        }

        /// <summary>
        /// Süresi geçen inputları temizler.
        /// </summary>
        private void CleanExpiredInputs()
        {
            while (buffer.Count > 0)
            {
                InputRecord oldest = buffer.Peek();
                
                if (Time.time - oldest.timestamp > bufferTime)
                {
                    buffer.Dequeue();
                    
                    if (showDebugInfo)
                    {
                        Debug.Log($"[InputBuffer] Expired: {oldest.type}");
                    }
                }
                else
                {
                    break; // Queue ordered olduğu için break yapabiliriz
                }
            }
        }

        /// <summary>
        /// Buffer'da kaç input var?
        /// </summary>
        public int BufferCount => buffer.Count;

        /// <summary>
        /// Buffer boş mu?
        /// </summary>
        public bool IsEmpty => buffer.Count == 0;
    }
}

