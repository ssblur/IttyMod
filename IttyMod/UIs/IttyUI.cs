using System.Collections.Generic;
using System;
using MLEM.Ui;
using MLEM.Ui.Elements;
using MLEM.Misc;
using MLEM.Textures;
using TinyLife.Objects;
using Vec2 = Microsoft.Xna.Framework.Vector2;
using Microsoft.Xna.Framework.Graphics;
using TinyLife.Uis;

namespace IttyMod.UIs
{
    public class IttyUI { 
        IttyButton button;
        IttyInterface menu;
        MLEM.Ui.RootElement root;
        
        class IttyButton : Button {
            IttyUI ui;
            public IttyButton(Anchor anchor, Vec2 size, IttyUI ittyUi) : base(anchor, size, "", "Open Itty to become sad") {
                Texture = new NinePatch(IttyMod.uiTextures[0, 0], 6, 6, 2, 2);
                OnPressed += Callback;
                ui = ittyUi;
            }

            public void Callback(Element element) {
                if(ui.menu != null && ui.root.System.Get("IttyUI") != null) {
                    ui.menu.Close();
                    ui.menu = null;
                } else {
                    ui.menu = new IttyInterface();
                    ui.root.System.Add("IttyUI", ui.menu);
                }
            }
        }

        class IttyInterface : CoveringGroup {
            Panel basePanel;
            Panel bitContainer;
            Image icon;
            Queue<Element> children = new Queue<Element>();

            public IttyInterface() : base(true, null, true, true) {
                basePanel = new Panel(Anchor.Center, new Vec2(0.666f, 0.666f), new Vec2(0.166f, 0.166f), false, false);
                AddChild(basePanel);

                icon = new Image(Anchor.TopLeft, new Vec2(16, 16), IttyMod.uiTextures[0, 0]);
                icon.Padding = new Padding(2, 2);
                basePanel.AddChild(icon);

                // title = new Paragraph(Anchor.TopCenter, 24, "Itty!");
                // title.TextScaleMultiplier = 2;
                // basePanel.AddChild(title);

                var group = new Group(Anchor.TopCenter, new Vec2(1, 1), false);
                group.ChildPadding = new Padding(5, 5, 24, 5);
                basePanel.AddChild(group);

                bitContainer = new Panel(Anchor.AutoCenter, new Vec2(1, 1), new Vec2(0, 0), false, true);
                group.AddChild(bitContainer);

                foreach(Bit b in BitManager.Load().Bits) {
                    AddBit(b);
                }
                BitManager.OnBitPublished += AddBit;
            }

            public void AddBit(Bit bit) {
                Panel panel = new Panel(Anchor.AutoCenter, new Vec2(0.95f, 0f), new Vec2(0, 0), true);
                Paragraph text = new Paragraph(Anchor.AutoRight, 130, bit.content);
                panel.AddChild(text, 0);

                var panelSize = new Vec2(0.475f, 1);
                float profileSize = 20;

                // Split tag groups into columns for formatting.
                Group group = new Group(Anchor.AutoCenter, new Vec2(1f, 1f), true);
                group.Padding = new MLEM.Misc.Padding(5, 5, 0, 0);
                Group leftColumn = new Group(Anchor.TopLeft, panelSize, true);
                Group rightColumn = new Group(Anchor.TopRight, panelSize, true);
                if(bit.creator != null){
                    Image image = new Image(Anchor.TopLeft, new Vec2(profileSize, profileSize), new TextureRegion(bit.creator.Portrait));
                    // Image image = new Image(Anchor.TopLeft, new Vec2(20, 20), IttyMod.uiTextures[1, 0]);
                    panel.AddChild(image);

                    Paragraph tag = new Paragraph(Anchor.AutoLeft, panelSize.X, String.Format("@{0}{1}", bit.creator.FirstName, bit.creator.LastName));
                    tag.TextColor = new Microsoft.Xna.Framework.Color(150, 150, 150);
                    leftColumn.AddChild(tag);
                } else {
                    Image image = new Image(Anchor.CenterLeft, new Vec2(profileSize, profileSize), IttyMod.uiTextures[1, 0]);
                    panel.AddChild(image);

                    Paragraph tag = new Paragraph(Anchor.AutoLeft, panelSize.X, String.Format("Sponsored"));
                    tag.TextColor = new Microsoft.Xna.Framework.Color(150, 150, 50);
                    leftColumn.AddChild(tag);
                }

                foreach(MapObject involved in bit.involved) {
                    if(involved is Person person) {
                        Paragraph tag = new Paragraph(Anchor.AutoRight, panelSize.X, String.Format("+@{0}{1}", person.FirstName, person.LastName));
                        tag.Alignment = MLEM.Formatting.TextAlignment.Right;
                        tag.TextColor = new Microsoft.Xna.Framework.Color(150, 150, 250);
                        rightColumn.AddChild(tag);
                    }
                }
                group.AddChild(leftColumn);
                group.AddChild(rightColumn);
                panel.AddChild(group);

                bitContainer.AddChild(panel, 0);

                children.Enqueue(panel);

                if(children.Count > 64) {
                    bitContainer.RemoveChild(children.Dequeue());
                }
            }

            public override void Draw(
                Microsoft.Xna.Framework.GameTime time, 
                SpriteBatch batch, 
                float alpha, 
                BlendState blendState, 
                SamplerState samplerState, 
                DepthStencilState depthStencilState, 
                Effect effect, 
                Microsoft.Xna.Framework.Matrix matrix
                )
            {
                base.Draw(time, batch, alpha, blendState, samplerState, depthStencilState, effect, matrix);
            }
        }

        public IttyUI(MLEM.Ui.RootElement element)
        {
            root = element;

            if(button != null){
                button.Dispose();
            }

            button = new IttyButton(
                Anchor.TopCenter,
                new Vec2(16, 16),
                this
            );

            element.System.Add("IttyButton", button);
        }

        public static void RootHandler(RootElement root)
        {
            if(root.Name == "InGameUi") {
                new IttyUI(root);
            }
        }
    }
}