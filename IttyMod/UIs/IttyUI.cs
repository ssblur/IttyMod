using System.Collections.Generic;
using System;
using MLEM.Ui;
using MLEM.Ui.Elements;
using MLEM.Textures;
using TinyLife.Objects;
using Vec2 = Microsoft.Xna.Framework.Vector2;
using Microsoft.Xna.Framework.Graphics;

namespace IttyMod.UIs
{
    public class IttyUI { 
        IttyButton button;
        IttyInterface menu;
        class IttyButton : Button {
            IttyUI ui;
            public IttyButton(Anchor anchor, Vec2 size, IttyUI ittyUi) : base(anchor, size, "", "Open Itty to become sad") {
                Texture = new NinePatch(IttyMod.uiTextures[0, 0], 6, 6, 2, 2);
                OnPressed += Callback;
                ui = ittyUi;
            }

            public void Callback(Element element) {
                ui.menu.Toggle();
            }
        }

        class IttyInterface : Element {
            Button closeButton;
            Panel basePanel;
            Paragraph title;
            Panel bitContainer;
            Queue<Element> children = new Queue<Element>();

            public IttyInterface() : base(Anchor.Center, new Vec2(204, 128)) {
                IsHidden = true;

                basePanel = new Panel(Anchor.TopLeft, Size, new Vec2(0, 0), false, false);
                AddChild(basePanel);

                title = new Paragraph(Anchor.AutoCenter, 24, "Itty!");
                title.TextScaleMultiplier = 2;
                basePanel.AddChild(title);

                bitContainer = new Panel(Anchor.AutoCenter, new Vec2(200, 102), new Vec2(0, 0), false, true, new Microsoft.Xna.Framework.Point(8, 12));
                basePanel.AddChild(bitContainer);

                closeButton = new Button(Anchor.TopRight, new Vec2(14, 14), "X");
                closeButton.OnPressed += OnClickClose;
                closeButton.Padding = new MLEM.Misc.Padding(2, 2, 2, 2);
                basePanel.AddChild(closeButton);

                foreach(Bit b in BitManager.INSTANCE.Bits) {
                    AddBit(b);
                }
                BitManager.INSTANCE.OnBitPublished += AddBit;
            }

            public void AddBit(Bit bit) {
                Panel panel = new Panel(Anchor.AutoCenter, new Vec2(170, 24), new Vec2(0, 0), true);
                Paragraph text = new Paragraph(Anchor.AutoRight, 130, bit.content);
                panel.AddChild(text, 0);

                if(bit.creator != null){
                    // Player portrait here eventually.
                    
                    Image image = new Image(Anchor.TopLeft, new Vec2(20, 20), IttyMod.uiTextures[1, 0]);
                    panel.AddChild(image);

                    Paragraph tag = new Paragraph(Anchor.AutoLeft, 50, String.Format("@{0}{1}", bit.creator.FirstName, bit.creator.LastName));
                    tag.TextColor = new Microsoft.Xna.Framework.Color(150, 150, 150);
                    panel.AddChild(tag);
                } else {
                    Image image = new Image(Anchor.CenterLeft, new Vec2(20, 20), IttyMod.uiTextures[1, 0]);
                    panel.AddChild(image);

                    Paragraph tag = new Paragraph(Anchor.AutoLeft, 50, String.Format("Sponsored", bit.creator.FirstName, bit.creator.LastName));
                    tag.TextColor = new Microsoft.Xna.Framework.Color(150, 150, 50);
                    panel.AddChild(tag);
                }

                foreach(MapObject involved in bit.involved) {
                    if(involved is Person) {
                        Paragraph tag = new Paragraph(Anchor.AutoRight, 50, String.Format("+@{0}{1}", ((Person) involved).FirstName, ((Person) involved).LastName));
                        tag.Alignment = MLEM.Formatting.TextAlignment.Right;
                        tag.TextColor = new Microsoft.Xna.Framework.Color(150, 150, 250);
                        panel.AddChild(tag);
                    }
                }

                bitContainer.AddChild(panel, 0);

                children.Enqueue(panel);

                if(children.Count > 64) {
                    bitContainer.RemoveChild(children.Dequeue());
                }
            }

            public void OnClickClose(Element element) {
                Toggle();
            }

            public void Toggle() {
                IsHidden = !IsHidden;
                CanBeMoused = !IsHidden;
            }

            public override void Draw(Microsoft.Xna.Framework.GameTime time, SpriteBatch batch, float alpha, BlendState blendState, SamplerState samplerState, Microsoft.Xna.Framework.Matrix matrix)
            {
                base.Draw(time, batch, alpha, blendState, samplerState, matrix);
            }
        }

        public IttyUI(MLEM.Ui.RootElement element)
        {
            menu = new IttyInterface();
            element.System.Add("IttyInterface", menu);

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