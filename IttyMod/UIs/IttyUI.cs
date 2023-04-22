using MLEM.Ui;
using Vec2 = Microsoft.Xna.Framework.Vector2;
using IttyMod.UIs.Components;

namespace IttyMod.UIs
{
    public class IttyUI { 
        public IttyInterface menu;
        public RootElement root;

        public IttyUI(){}

        public static void RootHandler(RootElement root)
        {
            if(root.Name == "InGameUi") {
                var panel = new IttyUI();
                var button = new IttyButton(panel);
                root.System.Add("IttyButton", button);
                panel.root = root;
            }
        }
    }
}