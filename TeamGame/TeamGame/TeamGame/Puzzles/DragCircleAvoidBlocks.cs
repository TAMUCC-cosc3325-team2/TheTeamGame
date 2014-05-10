using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lidgren.Network;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace TeamGame.Puzzles
{
    public enum BlockState
    {
        Waiting, 
        MovingLeft,
        MovingRight
    }

    public enum ClickedState
    {
        Clicked,
        Released
    }

    class DragCircleAvoidBlocks : IPuzzle
    {
        int blockMoveSpeed = 2;

        Rectangle backgroundRec, ball1Rec, ball2Rec, block1Rec, block2Rec;
        Texture2D background, ball, block1, block2;

        SoundEffectInstance prompt;

        Rectangle goalRec;
        Rectangle leftWall1, topWall1, bottomWall1, rightWall1, topPart1, bottomPart1;
        Rectangle leftWall2, topWall2, bottomWall2, rightWall2, topPart2, bottomPart2;
        MouseState mouse;

        public ClickedState ball1State
        {
            get;
            protected set;
        }

        public ClickedState ball2State
        {
            get;
            protected set;
        }

        public BlockState block1State
        {
            get;
            protected set;
        }

        public BlockState block2State
        {
            get;
            protected set;
        }

        public DragCircleAvoidBlocks(Game game, Player player)
            : base(game, player)
        {
            backgroundRec = new Rectangle(0, 0, 250, 150);
            goalRec = new Rectangle(90, 128, 22, 22);
            ball1Rec = new Rectangle(4, 0, 22, 22);
            ball2Rec = new Rectangle(223, 128, 22, 22);
            block1Rec = new Rectangle(220, 45, 70, 17);
            block2Rec = new Rectangle(200, 83, 50, 17);

            leftWall1 = new Rectangle(0, 0, 3, 141);
            topWall1 = new Rectangle(28, 11, 87, 3);
            rightWall1 = new Rectangle(114, 11, 3, 141);
            bottomWall1 = new Rectangle(0, 138, 86, 3);
            topPart1 = new Rectangle(28, 0, 3, 12);
            bottomPart1 = new Rectangle(86, 138, 3, 12);

            leftWall2 = new Rectangle(133, 0, 3, 141);
            topWall2 = new Rectangle(28 + 133, 11, 87, 3);
            rightWall2 = new Rectangle(114 + 133, 11, 3, 141);
            bottomWall2 = new Rectangle(0 + 133, 138, 86, 3);
            topPart2 = new Rectangle(28 + 133, 0, 3, 12);
            bottomPart2 = new Rectangle(86 + 133, 138, 3, 12);
            
            block1State = block2State = BlockState.Waiting;
            ball1State = ball2State = ClickedState.Released;
        }

        public override void Initialize()
        {

            background = Game.Content.Load<Texture2D>("art/dragCircleTest");
            ball = Game.Content.Load<Texture2D>("art/dragableBall");
            block1 = Game.Content.Load<Texture2D>("art/avoidBlock");
            block2 = Game.Content.Load<Texture2D>("art/avoidBlock");

            if (player == Game1.localPlayer)
            {
                prompt = Game.Content.Load<SoundEffect>("audio/DragTheCircles").CreateInstance();
                prompt.Play();
            }

            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, this.matrix);
            spriteBatch.Draw(background, backgroundRec, player.ColorOf());
            spriteBatch.Draw(block1, block1Rec, player.ColorOf());
            spriteBatch.Draw(block2, block2Rec, player.ColorOf());
            spriteBatch.Draw(ball, ball1Rec, player.ColorOf());
            spriteBatch.Draw(ball, ball2Rec, player.ColorOf());
            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            if (player != Game1.localPlayer)
                return; // this puzzle only updates by its owner

            mouse = Mouse.GetState();

            Vector2 relativePos = Mouse.GetState().Position() - (drawRegion.Location.ToVector2());

            if (Game1.random.Next(0, 100) % 25 == 0)
            {
                if (Game1.random.Next(0, 1) == 0 && block1State == BlockState.Waiting)
                {
                    if(block1Rec.Right == backgroundRec.Right)
                        block1State = BlockState.MovingLeft;
                    else
                        block1State = BlockState.MovingRight;
                }
                else if (block2State == BlockState.Waiting)
                {
                    if(block2Rec.Right == backgroundRec.Right)
                        block2State = BlockState.MovingLeft;
                    else
                        block2State = BlockState.MovingRight;
                }
            }

            if (block1State == BlockState.MovingLeft)
            {
                if (block1Rec.Left <= backgroundRec.Left)
                {
                    block1Rec.X = backgroundRec.Left;
                    block1State = BlockState.Waiting;
                }
                else
                    block1Rec.X -= blockMoveSpeed;
            }
            else if (block1State == BlockState.MovingRight)
            {
                if (block1Rec.Right >= backgroundRec.Right)
                {
                    block1Rec.X = backgroundRec.Right - block1Rec.Width;
                    block1State = BlockState.Waiting;
                }
                else
                    block1Rec.X += blockMoveSpeed;
            }
            if (block2State == BlockState.MovingLeft)
            {
                if (block2Rec.Left <= backgroundRec.Left)
                {
                    block2Rec.X = backgroundRec.Left;
                    block2State = BlockState.Waiting;
                }
                else
                    block2Rec.X -= blockMoveSpeed;
            }
            else if (block2State >= BlockState.MovingRight)
            {
                if (block2Rec.Right == backgroundRec.Right)
                {
                    block2Rec.X = backgroundRec.Right - block2Rec.Width;
                    block2State = BlockState.Waiting;
                }
                else
                    block2Rec.X += blockMoveSpeed;
            }


            if (mouse.LeftButton == ButtonState.Pressed && ball1Rec.Contains(relativePos))
                ball1State = ClickedState.Clicked;
            else if (mouse.LeftButton == ButtonState.Pressed && ball2Rec.Contains(relativePos))
                ball2State = ClickedState.Clicked;
            else
            {
                ball1Rec = new Rectangle(4, 0, 22, 22);
                ball2Rec = new Rectangle(223, 128, 22, 22);
            }

            if (ball1State == ClickedState.Clicked)
            {
                ball1Rec.Location = new Point((int)relativePos.X - 11, (int)relativePos.Y - 11);

                if (ball1Rec.Bottom >= bottomWall1.Top && ball1Rec.Left <= bottomPart1.Right)
                {
                    if (ball1Rec.Left <= leftWall1.Right)
                        ball1Rec.X = leftWall1.Right;
                    ball1Rec.Y = bottomWall1.Top - 22;
                }
                else if (ball1Rec.Left >= bottomPart1.Right && ball1Rec.Bottom >= backgroundRec.Bottom)
                    ball1Rec.Y = backgroundRec.Bottom - 22;
                else if (ball1Rec.Top <= topWall1.Bottom && ball1Rec.Right >= topPart1.Left)
                {
                    if (ball1Rec.Left <= rightWall1.Left)
                        ball1Rec.Y = topWall1.Bottom;
                }
                else if (ball1Rec.Right <= topPart1.Left && ball1Rec.Top <= backgroundRec.Top)
                    ball1Rec.Y = backgroundRec.Top;
                if (ball1Rec.Left <= leftWall1.Right)
                {
                    if (ball1Rec.Bottom >= bottomWall1.Top)
                        ball1Rec.Y = bottomWall1.Top - 22;
                    ball1Rec.X = leftWall1.Right;
                }
                else if (ball1Rec.Right >= rightWall1.Left)
                {
                    if (ball1Rec.Top <= topWall1.Bottom)
                        ball1Rec.Y = topWall1.Bottom;
                    ball1Rec.X = rightWall1.Left - 22;
                }

                ball2Rec.X = backgroundRec.Right - 22 - ball1Rec.X;
                ball2Rec.Y = backgroundRec.Bottom - 22 - ball1Rec.Y;

                if (mouse.LeftButton == ButtonState.Released)
                    ball1State = ClickedState.Released;
            }

            else if (ball2State == ClickedState.Clicked)
            {
                ball2Rec.Location = new Point((int)relativePos.X - 11, (int)relativePos.Y - 11);

                if (ball2Rec.Bottom >= bottomWall2.Top && ball2Rec.Left <= bottomPart2.Right)
                {
                    if (ball2Rec.Left <= leftWall2.Right)
                        ball2Rec.X = leftWall2.Right;
                    ball2Rec.Y = bottomWall2.Top - 22;
                }
                else if (ball2Rec.Left >= bottomPart2.Right && ball2Rec.Bottom >= backgroundRec.Bottom)
                    ball2Rec.Y = backgroundRec.Bottom - 22;
                else if (ball2Rec.Top <= topWall2.Bottom && ball2Rec.Right >= topPart2.Left)
                {
                    if (ball2Rec.Left <= rightWall2.Left)
                        ball2Rec.Y = topWall2.Bottom;
                }
                else if (ball2Rec.Right <= topPart2.Left && ball2Rec.Top <= backgroundRec.Top)
                    ball2Rec.Y = backgroundRec.Top;
                if (ball2Rec.Left <= leftWall2.Right)
                {
                    if (ball2Rec.Bottom >= bottomWall2.Top)
                        ball2Rec.Y = bottomWall2.Top - 22;
                    ball2Rec.X = leftWall2.Right;
                }
                else if (ball2Rec.Right >= rightWall2.Left)
                {
                    if (ball2Rec.Top <= topWall2.Bottom)
                        ball2Rec.Y = topWall2.Bottom;
                    ball2Rec.X = rightWall2.Left - 22;
                }

                ball1Rec.X = backgroundRec.Right - 22 - ball2Rec.X;
                ball1Rec.Y = backgroundRec.Bottom - 22 - ball2Rec.Y;

                if (mouse.LeftButton == ButtonState.Released)
                    ball2State = ClickedState.Released;
            }

            

            if (block1Rec.Intersects(ball1Rec) || block1Rec.Intersects(ball2Rec))
                PuzzleOver(false);
            else if (block2Rec.Intersects(ball1Rec) || block2Rec.Intersects(ball2Rec))
                PuzzleOver(false);

            if ((ball1Rec.Center.ToVector2() - goalRec.Center.ToVector2()).LengthSquared() <= 11)
                PuzzleOver(true);

        }

        public new void PuzzleOver(bool p)
        {
            if (prompt.State == SoundState.Playing)
                prompt.Dispose();
            if (p)
                Game.Content.Load<SoundEffect>("audio/Correct").Play(1.0f, 0.0f, 0.0f);
            else
                Game.Content.Load<SoundEffect>("audio/Incorrect").Play(1.0f, 0.0f, 0.0f);

            Game.Components.Remove(this);
            Game1.pStates[this.player].puzzle = new Puzzles.Transition(Game, player, p);  
        }

        public override void Encode(NetOutgoingMessage msg)
        {
            msg.Write((short)ball1Rec.X);
            msg.Write((short)ball1Rec.Y);
            msg.Write((short)ball2Rec.X); // can be computed from the ball1Pos
            msg.Write((short)ball2Rec.Y);
            msg.Write((short)block1Rec.X);
            msg.Write((short)block1Rec.Y);
            msg.Write((short)block2Rec.X);
            msg.Write((short)block2Rec.Y);
        }

        public override void Decode(NetIncomingMessage msg)
        {
            ball1Rec.X = msg.ReadInt16();
            ball1Rec.Y = msg.ReadInt16();
            ball2Rec.X = msg.ReadInt16();
            ball2Rec.Y = msg.ReadInt16();
            block1Rec.X = msg.ReadInt16();
            block1Rec.Y = msg.ReadInt16();
            block2Rec.X = msg.ReadInt16();
            block2Rec.Y = msg.ReadInt16();

        }
    }
}
