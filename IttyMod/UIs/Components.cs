using System.Collections.Generic;
using System;
using MLEM.Ui;
using MLEM.Ui.Elements;
using MLEM.Maths;
using MLEM.Textures;
using MLEM.Graphics;
using TinyLife.Uis;
using Vec2 = Microsoft.Xna.Framework.Vector2;
using TinyLife.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TinyLife;

namespace IttyMod.UIs.Components 
{
    public class BitPanel : Panel {
        Bit bit;

        public BitPanel(Bit bit): base(Anchor.AutoCenter, new Vec2(0.95f, 0f), new Vec2(0, 0), true) {
            this.bit = bit;

            var text = new Paragraph(Anchor.AutoRight, 130, bit.content);
            AddChild(text, 0);
            var panelSize = new Vec2(0.475f, 1);
            const float profileSize = 20;

            // Split tag groups into columns for formatting.
            var group = new Group(Anchor.AutoCenter, new Vec2(1f, 1f), true) {
                Padding = new Padding(5, 5, 0, 0)
            };
            var leftColumn = new Group(Anchor.TopLeft, panelSize, true);
            var rightColumn = new Group(Anchor.TopRight, panelSize, true);
            if(bit.creator != null){
                AddChild(
                    new Image(
                        Anchor.TopLeft, 
                        new Vec2(profileSize, profileSize),
                        new TextureRegion(bit.creator.Portrait)
                    )
                );

                var pronouns = "";
                if(bit.creator.Pronouns is { Length: > 0 })
                    pronouns = $"\n({bit.pronouns})";
                var location = $"\nfrom {bit.mapName}";
                var nameTag = bit.nameTag;
                nameTag = nameTag[..Math.Min(13, nameTag.Length)];
                var tag = new Paragraph(
                    Anchor.AutoLeft, 
                    panelSize.X,
                    $"{nameTag}{pronouns}{location}"
                ) {
                    TextScaleMultiplier = 0.9f,
                    TextColor = new Color(150, 150, 150)
                };
                leftColumn.AddChild(tag);
            } else {
                var image = new Image(Anchor.CenterLeft, new Vec2(profileSize, profileSize), IttyMod.UiTextures[0, 0]);
                AddChild(image);

                var tag = new Paragraph(Anchor.AutoLeft, panelSize.X, "Deactivated") {
                    TextColor = new Color(150, 150, 50)
                };
                leftColumn.AddChild(tag);
            }

            foreach(var involved in bit.involved) {
                if (involved is not Person person) continue;
                var nameTag = $"@{person.FirstName}{person.LastName}";
                nameTag = nameTag[..Math.Min(13, nameTag.Length)];
                var tag = new Paragraph(Anchor.AutoRight, panelSize.X, $"+{nameTag}") {
                    Alignment = MLEM.Formatting.TextAlignment.Right,
                    TextColor = new Color(150, 150, 250)
                };
                rightColumn.AddChild(tag);
            }
            group.AddChild(leftColumn);
            group.AddChild(rightColumn);
            AddChild(group);
        }

        public sealed override T AddChild<T>(T element, int index = -1)
        {
            return base.AddChild(element, index);
        }

        public override void Draw(GameTime time, SpriteBatch batch, float alpha, SpriteBatchContext context) {
            base.Draw(time, batch, alpha, context); 

            const int scale = 2;
            var activeIndex = 0;
            for(var i = 0; i < bit.reactions.Length; i++) {
                if(bit.reactions[i] == 0) continue;

                activeIndex++;
                var tex = IttyMod.UiTextures[i, 1, 1, 1];
                var pos = DisplayArea.Location;
                pos += new Vec2(DisplayArea.Size.X / 2f, this.DisplayArea.Size.Y - 12);
                pos -= new Vec2(-tex.Width * scale * (activeIndex - 2.5f), tex.Height * scale);

                batch.Draw(
                    tex, 
                    pos,
                    Color.White,
                    0,
                    Vec2.Zero,
                    new Vec2(scale, scale),
                    SpriteEffects.None,
                    0
                );
            }  

            // Always draw over all icons
            activeIndex = 0;
            for(var i = 0; i < bit.reactions.Length; i++) {
                if(bit.reactions[i] == 0) continue;

                activeIndex++;
                var tex = IttyMod.UiTextures[i, 1, 1, 1];
                var pos = this.DisplayArea.Location;
                pos += new Vec2(this.DisplayArea.Size.X / 2f, this.DisplayArea.Size.Y - 12);
                pos -= new Vec2(-tex.Width * scale * (activeIndex - 2.5f), tex.Height * scale);
                pos += new Vec2(20f, 10f);

                TinyLife.GameImpl.Instance.UiSystem.Style.Font.DrawString(
                    batch,
                    $"+{bit.reactions[i]}",
                    pos,
                    Color.Black,
                    0,
                    Vec2.Zero,
                    new Vec2(0.2f),
                    SpriteEffects.None,
                    0
                );
            }
        }
    }

    internal sealed class IttyButton : Button {
        IttyUI ui;
        public IttyButton(IttyUI ittyUi) : base(
            Anchor.AutoCenter, 
            new Vec2(14, 14), 
            "", 
            "Open Itty"
        ) {
            var image = new Image(Anchor.Center, new Vec2(14, 14), IttyMod.UiTextures[1, 0], true) {
                Padding = new Padding(3, 3)
            };
            AddChild(image);

            // Texture = new NinePatch(IttyMod.uiTextures[1, 0], 6, 6, 2, 2);
            OnPressed += Callback;
            ui = ittyUi;
            this.Padding = new Padding(0, 0);
        }

        public void Callback(Element element)
        {
            ui.menu?.Close();

            ui.menu = new IttyInterface();
            ui.root.System.Add("IttyUI", ui.menu);
        }
    }

    internal class LoadMoreButton : Button {
        public LoadMoreButton(GenericCallback callback) : base(Anchor.TopRight, new Vec2(20, 20), "", "") {
            Texture = new NinePatch(IttyMod.UiTextures[2, 2], 6, 6, 2, 2);
            OnPressed += callback;
        }
    }

    public class IttyInterface : CoveringGroup {
            Panel basePanel;
            Panel bitContainer;
            Group bitGroup;
            Image icon;
            Button loadMore;
            Queue<Element> children = new();
            Queue<Bit> newBits = new();

            public IttyInterface() : base(true, null, true, true) {
                basePanel = new Panel(Anchor.Center, new Vec2(0.666f, 0.666f), new Vec2(0.166f, 0.166f), false, false);
                AddChild(basePanel);

                icon = new Image(Anchor.TopLeft, new Vec2(72, 24), IttyMod.UiTextures[1, 0, 3, 1]) {
                    Padding = new Padding(2, 2)
                };
                basePanel.AddChild(icon);

                // title = new Paragraph(Anchor.TopCenter, 24, "Itty!");
                // title.TextScaleMultiplier = 2;
                // basePanel.AddChild(title);

                bitGroup = new Group(Anchor.TopCenter, new Vec2(1, 1), false) {
                    ChildPadding = new Padding(5, 5, 24, 5)
                };
                basePanel.AddChild(bitGroup);

                bitContainer = new Panel(Anchor.AutoCenter, new Vec2(1, 1), new Vec2(0, 0), false, true);
                bitGroup.AddChild(bitContainer);

                if(BitManager.Instance != null)
                    foreach(var b in BitManager.Instance.Bits) {
                        AddBit(b);
                    }
                BitManager.OnBitPublished += LoadBit;
            }

            public void LoadBit(Bit bit) {
                if(loadMore == null){    
                    loadMore = new LoadMoreButton(AddQueuedBits);
                    basePanel.AddChild(loadMore);
                }
                newBits.Enqueue(bit);
            }

            public void AddQueuedBits(Element element) {
                basePanel.RemoveChild(loadMore);
                loadMore = null;

                while(newBits.Count > 0)
                    AddBit(newBits.Dequeue());
            }

            public void AddBit(Bit bit) {
                Panel panel = new BitPanel(bit);
                bitContainer.AddChild(panel, 0);
                children.Enqueue(panel);

                if(children.Count > 64)
                    bitContainer.RemoveChild(children.Dequeue());
            }
        }
}