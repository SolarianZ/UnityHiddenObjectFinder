using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UObject = UnityEngine.Object;

namespace GBG.HiddenObjectFinder.Editor
{
    public class HiddenObjectInspectorElement : VisualElement
    {
        private InspectorElement _inspector;


        public HiddenObjectInspectorElement()
        {
            
        }

        public void SetTarget(UObject obj)
        {
            if (_inspector != null)
            {
                Remove(_inspector);
            }

            _inspector = new InspectorElement(obj)
            {
                name = "ObjectInspector",
                style =
                {
                    flexGrow = 1,
                },
            };
            Add(_inspector);
        }
    }
}
