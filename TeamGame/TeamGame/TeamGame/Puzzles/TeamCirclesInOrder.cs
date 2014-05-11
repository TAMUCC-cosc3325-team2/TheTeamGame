using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TeamGame.Puzzles
{


    /// <summary>
    /// This "team" puzzle is actually deceptively just four
    /// individual puzzles that all four teammates play at once
    /// </summary>
    class TeamCirclesInOrder : TPuzzle
    {
        byte cols = 14;
        byte rows = 8;
        byte buttonSize = 36;

        List<byte> buttons = new List<byte>(5);
        List<byte> buttonText = new List<byte>(5);
        bool clickedPrev = true;
        byte buttonClicked = 255;
        bool correct = false;

        public TeamCirclesInOrder(Game game, Player player)
            : base(game, player)
        {
            TeamCirclesInOrderHelper.nextToClick = 1;
        }

        public override void Initialize()
        {
            List<int> randList = new List<int>(Enumerable.Range(0, rows * cols / 4));
            for (int i = 0; i < 5; i++ )
            {
                int toAdd = randList[Game1.random.Next(0, randList.Count)];
                randList.Remove(toAdd);
                buttons.Add((byte) toAdd);
                buttonText.Add((byte) ((4 * i) + player.ID()));
            }

            base.Initialize();
        }

        private Vector2 buttonPos(byte bPos)
        {
            Vector2 offset = player.TeamOf().GetRegion().Location.ToVector2();
            if (bPos > 28)
                return new Vector2(-1000, -1000);
            bPos = (byte) ((4 * bPos) + player.ID() - 1);
            return new Vector2(buttonSize * (bPos % cols) + offset.X, buttonSize * ((byte) (bPos / cols)) + offset.Y);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Rectangle region = player.TeamOf().GetRegion();

            SpriteBatch spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            spriteBatch.Begin();
            for (int i = 0; i < buttons.Count; i++)
            {
                Vector2 position = buttonPos(buttons[i]);
                spriteBatch.DrawString(Game1.font, "" + buttonText[i], position + new Vector2(16, 16), Color.White, 0, Game1.font.MeasureString("" + buttonText[i]) / 2, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.Draw(Game.Content.Load<Texture2D>("art/circleAnimation"), position, new Rectangle(0, 0, 36, 36), player.ColorOf());
            }
            // spriteBatch.DrawString(Game1.font, " " + buttonClicked, Mouse.GetState().Position(), Color.White);
            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            if (!player.IsLocal()) // only check successes locally
                return;

            if (TeamCirclesInOrderHelper.nextToClick > 20)
            {
                PuzzleOver(true);
                return;
            }

            if (Mouse.GetState().LeftButton.IsClicked())
            {
                if (clickedPrev)
                    return;
                clickedPrev = true;

                List<byte> allButtons = new List<byte>(20);
                foreach (Player p in player.TeamList())
                    if (Game1.pStates[p].puzzle is Puzzles.TeamCirclesInOrder)
                        foreach (byte b in ((Puzzles.TeamCirclesInOrder) Game1.pStates[p].puzzle).buttons)
                            allButtons.Add(b);

                foreach (byte b in allButtons)
                {
                    if ((buttonPos(b) - (Mouse.GetState().Position() - player.TeamOf().GetRegion().Location.ToVector2() - new Vector2(16, 16))).LengthSquared() < 289)
                    {
                        if (b == this.buttons[0] && buttonText[0] == TeamCirclesInOrderHelper.nextToClick) // correct
                        {
                            buttons.RemoveAt(0);
                            buttonText.RemoveAt(0);
                            correct = true;
                        }
                        else // incorrect
                        {
                            buttonClicked = b; 
                            correct = false;
                        }
                        spawnAnimation(buttonPos(b), correct);
                        return;
                    }
                }
            }
            else // left mouse not clicked
                clickedPrev = false;

        }

        public override void Encode(Lidgren.Network.NetOutgoingMessage msg)
        {
            msg.Write((byte) TeamCirclesInOrderHelper.nextToClick);
            msg.Write(buttonClicked); // buttonClicked = 255;
            if (buttonClicked != 255)
                msg.Write(correct);
            foreach (byte b in buttons)
                msg.Write(b);
            for (int i = 5; i > buttons.Count; i--)
                msg.Write((byte)255);
        }
        public override void Decode(Lidgren.Network.NetIncomingMessage msg)
        {
            byte temp = msg.ReadByte();
            if (temp > TeamCirclesInOrderHelper.nextToClick)
                TeamCirclesInOrderHelper.nextToClick = temp;
            
            buttonClicked = msg.ReadByte();
            if (buttonClicked != 255)
            {
                spawnAnimation(buttonPos(buttonClicked), msg.ReadBoolean());
                buttonClicked = 255;
            }
            
            buttons = new List<byte>(5);
            for (int i = 0; i < 5; i++)
            {
                temp = msg.ReadByte();
                if (temp != 255)
                    buttons.Add(temp);
            }
        }

        private void spawnAnimation(Vector2 position, bool correct)
        {
            
        }

    }
    public static class TeamCirclesInOrderHelper
    {
        public static byte nextToClick;
    }

    public class mistakeAnimation : DrawableGameComponent
    {
        public mistakeAnimation(Game game, Vector2 position)
            : base(game)
        {

        }
    }
}
