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
        Texture2D cross, hexagon, square, trapeziod;
        MouseState mouse;
        List<Color> colorList;
        List<Texture2D> shapeList;
        List<Rectangle> positionList;
        Stopwatch stopWatch;
        Shape[] options = new Shape[4];
        Shape selected;
        Shape correct;
        Random random = new Random();

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
            positionList.Add(new Rectangle(drawRegion.Center.X - 75, drawRegion.Center.Y - 75, 50, 50));
            positionList.Add(new Rectangle(drawRegion.Center.X + 25, drawRegion.Center.Y - 75, 50, 50));
            positionList.Add(new Rectangle(drawRegion.Center.X - 75, drawRegion.Center.Y + 25, 50, 50));
            positionList.Add(new Rectangle(drawRegion.Center.X + 25, drawRegion.Center.Y + 25, 50, 50));

            //randomize attributes for each shape object
            for (int i = 0; i < 4; i++)
            {
                int[] index = { random.Next(colorList.Count), random.Next(shapeList.Count), random.Next(positionList.Count) };

                options[i] = new Shape(colorList[index[0]], shapeList[index[1]], positionList[index[2]]);
                
                //update attribute lists
                colorList.RemoveAt(index[0]);
                shapeList.RemoveAt(index[1]);
                positionList.RemoveAt(index[2]);
            }

            //set answer
            correct = options[random.Next(4)];

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

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (stopWatch.ElapsedMilliseconds > 5000)
            {
                mouse = Mouse.GetState();
                if (mouse.LeftButton == ButtonState.Pressed)
                { }             
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            
            spriteBatch.Begin();
            if (stopWatch.ElapsedMilliseconds < 5000)
            {
                for (int i = 0; i < 4; i++)
                {
                    spriteBatch.Draw(options[i].texture, options[i].position, options[i].color);
                }
            }
            if (stopWatch.ElapsedMilliseconds > 5000)
            {
                //ask for shape or color
            }

            spriteBatch.End();

            base.Draw(gameTime);
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
