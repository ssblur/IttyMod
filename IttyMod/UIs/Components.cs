using System.Collections.Generic;
using System;
using MLEM.Ui;
using MLEM.Ui.Elements;
using MLEM.Misc;
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

            Paragraph text = new Paragraph(Anchor.AutoRight, 130, bit.content);
            this.AddChild(text, 0);
            var panelSize = new Vec2(0.475f, 1);
            float profileSize = 20;

            // Split tag groups into columns for formatting.
            Group group = new Group(Anchor.AutoCenter, new Vec2(1f, 1f), true);
            group.Padding = new MLEM.Misc.Padding(5, 5, 0, 0);
            Group leftColumn = new Group(Anchor.TopLeft, panelSize, true);
            Group rightColumn = new Group(Anchor.TopRight, panelSize, true);
            if(bit.creator != null){
                Image image = new Image(Anchor.TopLeft, new Vec2(profileSize, profileSize), new TextureRegion(bit.creator.Portrait));
                this.AddChild(image);

                var pronouns = "";
                if(bit.creator.Pronouns != null && bit.creator.Pronouns.Length > 0)
                    pronouns = String.Format("\n({0})", bit.creator.Pronouns);
                var location = "";
                if(bit.creator.Map != GameImpl.Instance.CurrentMap)
                    location = String.Format("\nfrom {0}", bit.creator.Map.Info.Name);
                var nameTag = String.Format("@{0}{1}", bit.creator.FirstName, bit.creator.LastName);
                nameTag = nameTag.Substring(0, Math.Min(13, nameTag.Length));
                Paragraph tag = new Paragraph(
                    Anchor.AutoLeft, 
                    panelSize.X, 
                    String.Format("{0}{1}{2}", nameTag, pronouns, location)
                );
                tag.TextScaleMultiplier = 0.9f;
                tag.TextColor = new Microsoft.Xna.Framework.Color(150, 150, 150);
                leftColumn.AddChild(tag);
            } else {
                Image image = new Image(Anchor.CenterLeft, new Vec2(profileSize, profileSize), IttyMod.uiTextures[0, 0]);
                this.AddChild(image);

                Paragraph tag = new Paragraph(Anchor.AutoLeft, panelSize.X, String.Format("Deactivated"));
                tag.TextColor = new Microsoft.Xna.Framework.Color(150, 150, 50);
                leftColumn.AddChild(tag);
            }

            foreach(MapObject involved in bit.involved) {
                if(involved is Person person) {
                    var nameTag = String.Format("@{0}{1}", person.FirstName, person.LastName);
                    nameTag = nameTag.Substring(0, Math.Min(13, nameTag.Length));
                    Paragraph tag = new Paragraph(Anchor.AutoRight, panelSize.X, String.Format("+{0}", nameTag));
                    tag.Alignment = MLEM.Formatting.TextAlignment.Right;
                    tag.TextColor = new Microsoft.Xna.Framework.Color(150, 150, 250);
                    rightColumn.AddChild(tag);
                } else {
                    
                }
            }
            group.AddChild(leftColumn);
            group.AddChild(rightColumn);
            this.AddChild(group);
        }

        public override void Draw(GameTime time, SpriteBatch batch, float alpha, SpriteBatchContext context) {
            base.Draw(time, batch, alpha, context); 

            var scale = 2;
            var activeIndex = 0;
            for(int i = 0; i < bit.reactions.Length; i++) {
                if(bit.reactions[i] == 0) continue;

                activeIndex++;
                var tex = IttyMod.uiTextures[i, 1, 1, 1];
                var pos = this.DisplayArea.Location;
                pos += new Vec2(this.DisplayArea.Size.X / 2f, this.DisplayArea.Size.Y - 12);
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
            for(int i = 0; i < bit.reactions.Length; i++) {
                if(bit.reactions[i] == 0) continue;

                activeIndex++;
                var tex = IttyMod.uiTextures[i, 1, 1, 1];
                var pos = this.DisplayArea.Location;
                pos += new Vec2(this.DisplayArea.Size.X / 2f, this.DisplayArea.Size.Y - 12);
                pos -= new Vec2(-tex.Width * scale * (activeIndex - 2.5f), tex.Height * scale);
                pos += new Vec2(20f, 10f);

                TinyLife.GameImpl.Instance.UiSystem.Style.Font.DrawString(
                    batch,
                    String.Format("+{0}", bit.reactions[i]),
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
    
    class IttyButton : Button {
        IttyUI ui;
        public IttyButton(IttyUI ittyUi) : base(
            Anchor.AutoCenter, 
            new Vec2(14, 14), 
            "", 
            "Open Itty"
        ) {
            var image = new Image(Anchor.Center, new Vec2(14, 14), IttyMod.uiTextures[1, 0], true);
            image.Padding = new Padding(3, 3);
            AddChild(image);

            // Texture = new NinePatch(IttyMod.uiTextures[1, 0], 6, 6, 2, 2);
            OnPressed += Callback;
            ui = ittyUi;
            this.Padding = new Padding(0, 0);
        }

        public void Callback(Element element) {
            if(ui.menu != null) {
                ui.menu.Close();
                ui.menu = new IttyInterface();
                ui.root.System.Add("IttyUI", ui.menu);
            } else {
                ui.menu = new IttyInterface();
                ui.root.System.Add("IttyUI", ui.menu);
            }
        }
    }

    class LoadMoreButton : Button {
        public LoadMoreButton(GenericCallback Callback) : base(Anchor.TopRight, new Vec2(20, 20), "", "") {
            Texture = new NinePatch(IttyMod.uiTextures[2, 2], 6, 6, 2, 2);
            OnPressed += Callback;
        }
    }

    public class IttyInterface : CoveringGroup {
            Panel basePanel;
            Panel bitContainer;
            Group bitGroup;
            Image icon;
            Button loadMore;
            Queue<Element> children = new Queue<Element>();
            Queue<Bit> newBits = new Queue<Bit>();

            public IttyInterface() : base(true, null, true, true) {
                basePanel = new Panel(Anchor.Center, new Vec2(0.666f, 0.666f), new Vec2(0.166f, 0.166f), false, false);
                AddChild(basePanel);

                icon = new Image(Anchor.TopLeft, new Vec2(72, 24), IttyMod.uiTextures[1, 0, 3, 1]);
                icon.Padding = new Padding(2, 2);
                basePanel.AddChild(icon);

                // title = new Paragraph(Anchor.TopCenter, 24, "Itty!");
                // title.TextScaleMultiplier = 2;
                // basePanel.AddChild(title);

                bitGroup = new Group(Anchor.TopCenter, new Vec2(1, 1), false);
                bitGroup.ChildPadding = new Padding(5, 5, 24, 5);
                basePanel.AddChild(bitGroup);

                bitContainer = new Panel(Anchor.AutoCenter, new Vec2(1, 1), new Vec2(0, 0), false, true);
                bitGroup.AddChild(bitContainer);

                if(BitManager.INSTANCE != null)
                    foreach(Bit b in BitManager.INSTANCE.Bits) {
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

            public override void Draw(GameTime time, SpriteBatch batch, float alpha, SpriteBatchContext context)
            {
                base.Draw(time, batch, alpha, context);
            }
        }
}