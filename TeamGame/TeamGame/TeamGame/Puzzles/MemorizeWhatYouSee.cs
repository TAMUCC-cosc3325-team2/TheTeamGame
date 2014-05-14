using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using System.Diagnostics;

namespace TeamGame.Puzzles
{
    class MemorizeWhatYouSee : IPuzzle
    {
        Texture2D cross, hexagon, square, trapeziod, ball;
        MouseState mouse;
        List<Color> colorList;
        List<Texture2D> shapeList;
        List<Rectangle> positionList;
        Stopwatch stopWatch;
        Shape[] options = new Shape[4];
        Color selected;
        Shape correct;
        SoundEffectInstance memorizeIntro, memorizePrompt;
        Boolean prompt;
        //Random random = new Random();

        public MemorizeWhatYouSee(Game game, Player player)
            : base(game, player)
        {
            //attribute lists
            colorList = new List<Color>();
            colorList.Add(Color.Red);
            colorList.Add(Color.Green);
            colorList.Add(Color.Blue);
            colorList.Add(Color.Yellow);
            
            shapeList = new List<Texture2D>();
            shapeList.Add(cross);
            shapeList.Add(hexagon);
            shapeList.Add(square);
            shapeList.Add(trapeziod);

            positionList = new List<Rectangle>();
            positionList.Add(new Rectangle(drawRegion.Center.X - 63, drawRegion.Center.Y - 63, 50, 50));
            positionList.Add(new Rectangle(drawRegion.Center.X + 13, drawRegion.Center.Y - 63, 50, 50));
            positionList.Add(new Rectangle(drawRegion.Center.X - 63, drawRegion.Center.Y + 13, 50, 50));
            positionList.Add(new Rectangle(drawRegion.Center.X + 13, drawRegion.Center.Y + 13, 50, 50));

            //randomize attributes for each shape object
            for (int i = 0; i < 4; i++)
            {
                int[] index = { Game1.random.Next(colorList.Count), Game1.random.Next(shapeList.Count), Game1.random.Next(positionList.Count) };

                options[i] = new Shape(colorList[index[0]], shapeList[index[1]], positionList[index[2]]);
                
                //update attribute lists
                colorList.RemoveAt(index[0]);
                shapeList.RemoveAt(index[1]);
                positionList.RemoveAt(index[2]);
            }

            //initialize boolean to check if prompt audio clip needs to play
            prompt = true;

            //set answer
            correct = options[Game1.random.Next(4)];

            //start stopwatch
            stopWatch = new Stopwatch();
            stopWatch.Start();
        }

        public override void Initialize()
        {
            cross = Game.Content.Load<Texture2D>("art/cross");
            hexagon = Game.Content.Load<Texture2D>("art/hexagon");
            square = Game.Content.Load<Texture2D>("art/square");
            trapeziod = Game.Content.Load<Texture2D>("art/trapeziod");
            ball = Game.Content.Load<Texture2D>("art/ball50px");

            if (player == Game1.localPlayer)
            {
                memorizeIntro = Game.Content.Load<SoundEffect>("audio/MemorizeWhatYouSee").CreateInstance();
                memorizePrompt = Game.Content.Load<SoundEffect>("audio/WhichColorWasTheShape").CreateInstance();
                memorizeIntro.Play();
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (player != Game1.localPlayer)
                return; // this puzzle only updates by its owner

            //after 5 seconds, player can select color
            if (stopWatch.ElapsedMilliseconds > 7000)
            {
                if (prompt)
                {
                    memorizePrompt.Play();
                    prompt = false;
                }

                mouse = Mouse.GetState();
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    if (new Rectangle(drawRegion.Center.X - 100, drawRegion.Center.Y + 13, 50, 50).Contains(mouse.Position()))
                        selected = Color.Red;
                    if (new Rectangle(drawRegion.Center.X - 50, drawRegion.Center.Y + 13, 50, 50).Contains(mouse.Position()))
                        selected = Color.Green;
                    if (new Rectangle(drawRegion.Center.X, drawRegion.Center.Y + 13, 50, 50).Contains(mouse.Position()))
                        selected = Color.Blue;
                    if (new Rectangle(drawRegion.Center.X + 50, drawRegion.Center.Y + 13, 50, 50).Contains(mouse.Position()))
                        selected = Color.Yellow;

                    if (selected == correct.color)
                    {
                        PuzzleOver(true);
                    }
                    else
                    {
                        PuzzleOver(false);
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            
            //show shapes
            spriteBatch.Begin();
            if (stopWatch.ElapsedMilliseconds < 7000)
            {
                for (int i = 0; i < 4; i++)
                {
                    spriteBatch.Draw(options[i].texture, options[i].position, options[i].color);
                }
            }
            //after 5 seconds, show question
            if (stopWatch.ElapsedMilliseconds > 7000)
            {
                //show shape
                spriteBatch.Draw(correct.texture, new Rectangle(drawRegion.Center.X - 25, drawRegion.Center.Y - 63, 50, 50), Color.White);
                //show color choices
                spriteBatch.Draw(ball, new Rectangle(drawRegion.Center.X - 100, drawRegion.Center.Y + 13, 50, 50), Color.Red);
                spriteBatch.Draw(ball, new Rectangle(drawRegion.Center.X - 50, drawRegion.Center.Y + 13, 50, 50), Color.Green);
                spriteBatch.Draw(ball, new Rectangle(drawRegion.Center.X, drawRegion.Center.Y + 13, 50, 50), Color.Blue);
                spriteBatch.Draw(ball, new Rectangle(drawRegion.Center.X + 50, drawRegion.Center.Y + 13, 50, 50), Color.Yellow);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public override void Encode(Lidgren.Network.NetOutgoingMessage msg)
        {
            //msg.Write((byte)timesDisplayed);
            //msg.Write((byte)ballColor);
            //msg.Write((byte)textColor);
            //msg.Write((byte)textWord);
        }

        public override void Decode(Lidgren.Network.NetIncomingMessage msg)
        {
            //timesDisplayed = msg.ReadByte();
            //ballColor = (MyColor)msg.ReadByte();
            //textColor = (MyColor)msg.ReadByte();
            //textWord = (MyColor)msg.ReadByte();
        }

        public override void PuzzleOver(bool p)
        {
            if (memorizePrompt.State == SoundState.Playing)
                memorizePrompt.Dispose();
            if (p)
                Game.Content.Load<SoundEffect>("audio/Correct").Play(1.0f, 0.0f, 0.0f);
            else
                Game.Content.Load<SoundEffect>("audio/Incorrect").Play(1.0f, 0.0f, 0.0f);

            Game.Components.Remove(this);

            Game1.pStates[this.player].puzzle = new Puzzles.Transition(Game, player, p);
        }
    }

    public class Shape
    {
        public Color color;
        public Texture2D texture;
        public Rectangle position;

        public Shape(Color c, Texture2D t, Rectangle p)
        {
            color = c;
            texture = t;
            position = p;
        }
    }
}
