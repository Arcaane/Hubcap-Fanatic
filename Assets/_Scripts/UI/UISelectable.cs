using UnityEngine;

namespace HubcapInterface {
    public class UISelectable : MonoBehaviour {
        [SerializeField] private UISelectable northSelectable = null;
        [SerializeField] private UISelectable eastSelectable = null;
        [SerializeField] private UISelectable southSelectable = null;
        [SerializeField] private UISelectable westSelectable = null;
        private bool isElementSelected = false;

        public virtual void DisableSelection() => isElementSelected = false;

        public virtual void EnableSelection() {
            isElementSelected = true;
            MainMenuManager.Instance.UpdateSelectedObj(this);
        }

        public virtual void ActionInputPressed() {}

        /// <summary>
        /// Method called to switch selectable element when an input is recieved
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public bool TryGoInDirection(Direction dir) {
            UISelectable selectableToCheck = dir switch {
                Direction.North => northSelectable,
                Direction.South => southSelectable,
                Direction.East => eastSelectable,
                Direction.West => westSelectable,
                _ => null
            };

            if (selectableToCheck == null) return false;
            DisableSelection();
            selectableToCheck.EnableSelection();
            return true;
        }
    }
}
