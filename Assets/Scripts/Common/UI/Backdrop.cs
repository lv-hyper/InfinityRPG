using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UI
{
    
    public class Backdrop : MonoBehaviour
    {
        public interface ILockable
        {
            string LockId { get; }
            string LockMessage { get; }
            float Progress { get; }
        }
        
        [SerializeField] private TMP_Text messageText;
        
        private Dictionary<string, ILockable> activeLocks = new Dictionary<string, ILockable>();

        public bool ContainsLock(ILockable lockable)
        {
            return activeLocks.ContainsKey(lockable.LockId);
        }

        public void AddLock(ILockable lockable)
        {
            if (!activeLocks.ContainsKey(lockable.LockId))
            {
                activeLocks[lockable.LockId] = lockable;
                UpdateBackdropState();
            }
        }

        public void RemoveLock(ILockable lockable)
        {
            if (activeLocks.ContainsKey(lockable.LockId))
            {
                activeLocks.Remove(lockable.LockId);
                UpdateBackdropState();
            }
        }

        public void UpdateLock(ILockable lockable)
        {
            UpdateBackdropState();
        }
        
        private void UpdateBackdropState()
        {
            if (activeLocks.Count > 0)
            {
                ShowBackdrop();
            }
            else
            {
                HideBackdrop();
            }
        }
        
        private void ShowBackdrop()
        {
            gameObject.SetActive(true);

            // Get the lock with the highest progress to display
            var displayLock = activeLocks.Values
                .OrderByDescending(l => l.Progress)
                .FirstOrDefault();
            messageText.text = displayLock.LockMessage;
        }

        private void HideBackdrop()
        {
            gameObject.SetActive(false);
        }
        
    }
}