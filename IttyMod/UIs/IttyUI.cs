using MLEM.Ui;
using System;
using Vec2 = Microsoft.Xna.Framework.Vector2;
using IttyMod.UIs.Components;
using MLEM.Ui.Elements;

namespace IttyMod.UIs
{
    public class IttyUI { 
        public IttyInterface menu;
        public Element root;

        public IttyUI(){}

        public static void RootHandler(RootElement root)
        {
            if(root.Name == "InGameUi") {
                int i = 0;
                root.OnElementAdded += element => {
                    if(element is Panel) {
                        if(i == 0) {
                            i++;
                            var panel = new IttyUI();
                            panel.root = element;

                            var group = new Group(Anchor.AutoLeft, new Vec2(16, 14));
                            var button = new IttyButton(panel);
                            group.AddChild(button);
                        
                            element.AddChild(group);
                        }
                        i++;
                    }
                };
            }
            
        }
    }
}