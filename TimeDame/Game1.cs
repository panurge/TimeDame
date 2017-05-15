
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using Shooter;
using System;
using System.Web;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Timers;
using TimeDame;
//using System.Data.SqlClient;


namespace Game2
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        //Player player;
        private SpriteFont font1, font2, font3;
        TimeSpan tSpan;
        DateTime startTime;
        static int numCand = 5;
        DateTime[] clotsLast = new DateTime[numCand];
        TimeSpan[] clots = new TimeSpan[numCand];
        TimeSpan[] clotsPartTotal = new TimeSpan[numCand];
        TimeSpan[] clotsTotal = new TimeSpan[numCand];
        TimeSpan totalTime;

        StopWatchWithOffset[] swTotals = new StopWatchWithOffset[numCand];
        StopWatchWithOffset[] swPartTotals = new StopWatchWithOffset[numCand];

        string[] codgers = { "Andrew", "Kirsty", "Carwyn", "Leanne", "Neil", "ploo" }; //new string[5];
        string[] geezers = { "Carwyn", "Andrew", "Leanne", "Neil", "Mark" };

        bool[] count = { false, false, false, false, false, false };
        string[] percs = { "0%", "0%", "0%", "0%", "0%", "0%" };
        int h = 400;
        int ht = 1024;
        int wd = 1920;
        public int clockh = -20;
        string cl0;
        KeyboardState previousState;
        bool countdown = false;
        TimeSpan ploo = new TimeSpan(0); //= new System.DateTime();

        public Color clockColor = Color.White;

        public bool inload = false; //{ get; private set; }
        public string temptime { get; private set; }
        public Texture2D mugshot { get; private set; }
        public bool logoOnOff = true;

        public bool clockOnOff = true;
        public bool fullscreen = false;

        public string counterduration = "00:00:45"; //{ get; private set; }
        private Texture2D itvlogo;

        public Game1()
        {
            Load_ScreenRes();
            graphics = new GraphicsDeviceManager(this);
            //graphics.ToggleFullScreen();
            //
            graphics.PreferredBackBufferWidth = wd;
            graphics.PreferredBackBufferHeight = ht;
           // wd = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
           // ht = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            //graphics.ToggleFullScreen();

            //graphics.PreferredBackBufferWidth = wd;
            //graphics.PreferredBackBufferHeight = ht;
            h = (5 * ht / 8) - 20;
            //	graphics.PreferredBackBufferWidth = game.GraphicsDevice.Viewport.Width;
            //	graphics.PreferredBackBufferHeight = game.GraphicsDevice.Viewport.Height;

            //	graphics.PreferredBackBufferWidth = Game.GraphicsDevice.Viewport.Width;
            //	graphics.PreferredBackBufferHeight = Game.GraphicsDevice.Viewport.Height;


            Content.RootDirectory = "Content";

            this.Window.Title = "TimeLady";
            //this.Window.Title = "Time Lady Hot Air Calculator by Keith Jones";

        }

        private void sendMail()
        {
            string message = "";

            //message = "Part Time " + DateTime.Now + "\r\n";
            message += "<table style=\"font-size: 200%;border-collapse: collapse;border: 1px solid black; border-spacing: 10px;\">";
            for (int i = 0; i < numCand; i++)
            {

                message += "<tr style=\"border: 1px solid black\"><td cellpadding = \"10\" style=\"border: 1px solid black\">" + geezers[i] + "</td><td> ";
                message += swTotals[i].ElapsedTimeSpan.ToString(@"hh\:mm\:ss") + "</td><td cellpadding = \"10\" style=\"border: 1px solid black\"> ";// = new StopWatchWithOffset(TimeSpan.FromSeconds(0));
                message += swPartTotals[i].ElapsedTimeSpan.ToString(@"hh\:mm\:ss") + "</td><td cellpadding = \"10\" style=\"border: 1px solid black\"> ";  //= new StopWatchWithOffset(TimeSpan.FromSeconds(0));
                message +=  (percs[i].ToString())  + "</td></tr>";
            }
            message += "</table>";
            SmtpClient smtpClient = new SmtpClient
            {
                Host = "10.211.6.226",
                Port = 25,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = System.Net.CredentialCache.DefaultNetworkCredentials
            };

            MailMessage mailMessage = new MailMessage
            {
                Body = message,
                From = new MailAddress("keith.jones@itv.com"),
                Subject = "ITV Wales Leaders Debate part times " + DateTime.Now, //ToString(@"hh\:mm\:ss"),
                Priority = MailPriority.Normal,
                IsBodyHtml = true

            };

            mailMessage.To.Add("keith.jones@itv.com");
            mailMessage.To.Add("Lynwen.James@itv.com");

            smtpClient.Send(mailMessage);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            try
            {
                Xkeys xkey = new Xkeys();
                Xkeys.BLAllOff();
            }
            catch { }

            for (int i = 0; i < numCand; i++)
            {
                swTotals[i] = new StopWatchWithOffset(TimeSpan.FromSeconds(0));
                swPartTotals[i] = new StopWatchWithOffset(TimeSpan.FromSeconds(0));
            }
            // TODO: Add your initialization logic here
            GraphicsDevice.PresentationParameters.BackBufferFormat = SurfaceFormat.Alpha8;
            startTime = DateTime.Now + TimeSpan.Parse("00:00:45");
            base.Initialize();

            //player = new Shooter.Player();
            totalTime = TimeSpan.Parse("00:00:00:00");
            // clots[1] = TimeSpan.Parse("00:05:23");
            //clots[2] = TimeSpan.Parse("00:03:55");
            previousState = Keyboard.GetState();
            resetClock();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
            font1 = Content.Load<SpriteFont>("Graphics\\timefontfont");
            font2 = Content.Load<SpriteFont>("Graphics\\File");
            font3 = Content.Load<SpriteFont>("Graphics\\Font3");
            mugshot = Content.Load<Texture2D>("Graphics\\mugshotTransparent200");

            itvlogo = Content.Load<Texture2D>("Graphics\\itv-Cymru-Wales-RGB-MultiColour-Neg"); // change these names to the names of your images


            // Use the name of your sprite font file here instead of 'Score'.
            //player.Initialize(Content.Load<Texture2D>("Graphics\\player"), playerPosition);
        }
        //C:\Users\Keith\Download
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)))
            {
                saveState();
                Exit();
            }
            if (!inload)
            {

                if (state.IsKeyDown(Keys.NumPad0) & !previousState.IsKeyDown(Keys.NumPad0)) // & state.IsKeyDown(Keys.LeftAlt)
                {
                    temptime += "0";

                }
                if (state.IsKeyDown(Keys.NumPad1) & !previousState.IsKeyDown(Keys.NumPad1)) // & state.IsKeyDown(Keys.LeftAlt)
                {
                    temptime += "1";

                }
                if (state.IsKeyDown(Keys.NumPad2) & !previousState.IsKeyDown(Keys.NumPad2)) // & state.IsKeyDown(Keys.LeftAlt)
                {
                    temptime += "2";

                }
                if (state.IsKeyDown(Keys.NumPad3) & !previousState.IsKeyDown(Keys.NumPad3)) // & state.IsKeyDown(Keys.LeftAlt)
                {
                    temptime += "3";

                }
                if (state.IsKeyDown(Keys.NumPad4) & !previousState.IsKeyDown(Keys.NumPad4)) // & state.IsKeyDown(Keys.LeftAlt)
                {
                    temptime += "4";

                }
                if (state.IsKeyDown(Keys.NumPad5) & !previousState.IsKeyDown(Keys.NumPad5)) // & state.IsKeyDown(Keys.LeftAlt)
                {
                    temptime += "5";

                }
                if (state.IsKeyDown(Keys.NumPad6) & !previousState.IsKeyDown(Keys.NumPad6)) // & state.IsKeyDown(Keys.LeftAlt)
                {
                    temptime += "6";

                }
                if (state.IsKeyDown(Keys.NumPad7) & !previousState.IsKeyDown(Keys.NumPad7)) // & state.IsKeyDown(Keys.LeftAlt)
                {
                    temptime += "7";

                }
                if (state.IsKeyDown(Keys.NumPad8) & !previousState.IsKeyDown(Keys.NumPad8)) // & state.IsKeyDown(Keys.LeftAlt)
                {
                    temptime += "8";

                }
                if (state.IsKeyDown(Keys.NumPad9) & !previousState.IsKeyDown(Keys.NumPad9)) // & state.IsKeyDown(Keys.LeftAlt)
                {
                    temptime += "9";

                }
                if (state.IsKeyDown(Keys.Enter) & !previousState.IsKeyDown(Keys.Enter)) // & state.IsKeyDown(Keys.LeftAlt)
                {
                    Debug.WriteLine(temptime);
                    try
                    {
                        int l = temptime.Length;
                        char[] chars = temptime.ToCharArray();
                        counterduration = "00:";
                        counterduration += chars[l - 4].ToString();
                        counterduration += chars[l - 3].ToString();
                        counterduration += ":";
                        counterduration += chars[l - 2].ToString();
                        counterduration += chars[l - 1].ToString();

                        temptime = "";
                        countdown = false;
                        clockColor = Color.White;
                        ploo = TimeSpan.Parse(counterduration);
                        resetClock();

                    }
                    catch { }
                    Debug.WriteLine(counterduration);

                }
                if (state.IsKeyDown(Keys.LeftControl) & state.IsKeyDown(Keys.R) & !previousState.IsKeyDown(Keys.R)) // & state.IsKeyDown(Keys.LeftAlt)
                {
                    Debug.WriteLine("Reset");
                    for (int i = 0; i < numCand; i++)
                    {
                        swTotals[i] = new StopWatchWithOffset(TimeSpan.FromSeconds(0));
                        swPartTotals[i] = new StopWatchWithOffset(TimeSpan.FromSeconds(0));
                        percs[i] = "0%";

                    }
                    totalTime = TimeSpan.Parse("0");
                }


                if (state.IsKeyDown(Keys.LeftControl) & state.IsKeyDown(Keys.P) & !previousState.IsKeyDown(Keys.P)) // & state.IsKeyDown(Keys.LeftAlt)
                {
                    Debug.WriteLine("Prog Part");
                    sendMail();
                    for (int i = 0; i < numCand; i++)
                    {
                        //swTotals[i] = new StopWatchWithOffset(TimeSpan.FromSeconds(0));
                        swPartTotals[i] = new StopWatchWithOffset(TimeSpan.FromSeconds(0));
                        // percs[i] = "0%";

                    }

                    // totalTime = TimeSpan.Parse("0");
                }
                if (state.IsKeyDown(Keys.LeftControl) & state.IsKeyDown(Keys.K) & !previousState.IsKeyDown(Keys.K)) // & state.IsKeyDown(Keys.LeftAlt)
                {
                    Debug.WriteLine("Clock On/Off");
                    clockOnOff = !clockOnOff;
                  
                }



                if (state.IsKeyDown(Keys.LeftControl) & state.IsKeyDown(Keys.L) & !previousState.IsKeyDown(Keys.L)) // & state.IsKeyDown(Keys.LeftAlt)
                {
                    loadState();

                }
                if (state.IsKeyDown(Keys.LeftControl) & state.IsKeyDown(Keys.I) & !previousState.IsKeyDown(Keys.I)) // & state.IsKeyDown(Keys.LeftAlt)
                {
                    logoOnOff = !logoOnOff;

                }
                if (state.IsKeyDown(Keys.LeftControl) & state.IsKeyDown(Keys.T) & !previousState.IsKeyDown(Keys.T)) // & state.IsKeyDown(Keys.LeftAlt)
                {
                    Debug.WriteLine("Cntrl+T");
                    saveState();
                    loadTotals();

                }
                if (state.IsKeyDown(Keys.LeftControl) & state.IsKeyDown(Keys.F) & !previousState.IsKeyDown(Keys.F)) // & state.IsKeyDown(Keys.LeftAlt)
                {
                    if (fullscreen == true)
                    {
                        h = 500;
                        clockh = -20;
                        graphics.PreferredBackBufferWidth = 1600;
                        graphics.PreferredBackBufferHeight = 800;
                        graphics.IsFullScreen = false;
                        fullscreen = false;
                    }
                    else
                    {
                        h = 700;
                        clockh = 70;
                        graphics.PreferredBackBufferWidth = 1600;
                        graphics.PreferredBackBufferHeight = 1024;
                        graphics.IsFullScreen = true;
                        fullscreen = true;
                    }
                    graphics.ApplyChanges();
                }
                //if (state.IsKeyDown(Keys.LeftControl) & state.IsKeyDown(Keys.S) & !previousState.IsKeyDown(Keys.S)) // & state.IsKeyDown(Keys.LeftAlt)
                //    Debug.WriteLine("Save last state");


                //Individual countdown timers adjusted on each keypress 1-5

                if (!state.IsKeyDown(Keys.LeftControl) & state.IsKeyDown(Keys.D1) & !previousState.IsKeyDown(Keys.D1))
                {
                    count[0] = !count[0];
                    if (count[0])
                    {
                        swTotals[0].Startw();
                        swPartTotals[0].Startw();
                        Xkeys.BLOn(0);
                    }
                    else
                    {
                        swTotals[0].Stopw();
                        swPartTotals[0].Stopw();
                        saveState();
                        Xkeys.BLOff(0);
                    }
                    //print_state();
                }

                //if (count[0]) clots[0] = (DateTime.Now.Subtract(clotsLast[0]));//.Add(clotsTotal[0]);
                //clotsPartTotal[0] = clotsTotal[0].Add(clots[0]);

                if (!state.IsKeyDown(Keys.LeftControl) & state.IsKeyDown(Keys.D2) & !previousState.IsKeyDown(Keys.D2))
                {
                    count[1] = !count[1];
                    if (count[1])
                    {
                        swTotals[1].Startw();
                        swPartTotals[1].Startw();
                        Xkeys.BLOn(1);
                    }
                    else
                    {
                        swTotals[1].Stopw();
                        swPartTotals[1].Stopw();
                        saveState();
                        Xkeys.BLOff(1);
                    }
                    //print_state();
                }

                //if (count[1]) clots[1] = (DateTime.Now.Subtract(clotsLast[1]));//.Add(clotsTotal[1]);

                if (!state.IsKeyDown(Keys.LeftControl) & state.IsKeyDown(Keys.D3) & !previousState.IsKeyDown(Keys.D3))
                {
                    count[2] = !count[2];
                    if (count[2])
                    {
                        swTotals[2].Startw();
                        swPartTotals[2].Startw();
                        Xkeys.BLOn(2);
                    }
                    else
                    {
                        swTotals[2].Stopw();
                        swPartTotals[2].Stopw();
                        saveState();
                        Xkeys.BLOff(2);
                    }
                    //print_state();
                }
                // if (count[2]) clots[2] = (DateTime.Now.Subtract(clotsLast[2]));//.Add(clotsTotal[2]);


                if (!state.IsKeyDown(Keys.LeftControl) & state.IsKeyDown(Keys.D4) & !previousState.IsKeyDown(Keys.D4))
                {
                    count[3] = !count[3];
                    if (count[3])
                    {
                        swTotals[3].Startw();
                        swPartTotals[3].Startw();
                        Xkeys.BLOn(3);
                    }
                    else
                    {
                        swTotals[3].Stopw();
                        swPartTotals[3].Stopw();
                        saveState();
                        Xkeys.BLOff(3);
                    }
                }
                //print_state();
                //if (count[3]) clots[3] = (DateTime.Now.Subtract(clotsLast[3]));//.Add(clotsTotal[3]);


                if (!state.IsKeyDown(Keys.LeftControl) & state.IsKeyDown(Keys.D5) & !previousState.IsKeyDown(Keys.D5))
                {
                    count[4] = !count[4];
                    if (count[4])
                    {
                        swTotals[4].Startw();
                        swPartTotals[4].Startw();
                        Xkeys.BLOn(4);
                    }
                    else
                    {
                        swTotals[4].Stopw();
                        swPartTotals[4].Stopw();
                        saveState();
                        Xkeys.BLOff(4);
                    }
                    //print_state();
                }

                if (!state.IsKeyDown(Keys.LeftControl) & state.IsKeyDown(Keys.D6) & !previousState.IsKeyDown(Keys.D6))
                {
                    count[5] = !count[5];
                    if (count[5])
                    {
                        swTotals[5].Startw();
                        swPartTotals[5].Startw();
                        Xkeys.BLOn(5);
                    }
                    else
                    {
                        swTotals[5].Stopw();
                        swPartTotals[5].Stopw();
                        saveState();
                        Xkeys.BLOff(5);
                    }
                    //print_state();
                }
                //§§ if (count[4]) clots[4] = (DateTime.Now.Subtract(clotsLast[4]));//1.Add(clotsTotal[4]);

                totalTime = TimeSpan.Parse("00:00:00:00");
                for (int i = 0; i < numCand; i++) totalTime += swTotals[i].ElapsedTimeSpan;
                //Debug.WriteLine(totalTime.Ticks.ToString());
                // TODO: Add your update logic here
                for (int i = 0; i < numCand; i++)
                {
                    if (totalTime.TotalMilliseconds > 0) percs[i] = Math.Round(((((swTotals[i]).ElapsedTimeSpan.TotalMilliseconds) * 1000) / ((totalTime.TotalMilliseconds))) / 10f).ToString() + "%";
                    //Debug.WriteLine(i + ", " + percs[i] + ", " + swTotals[i].ElapsedTimeSpan.TotalMilliseconds.ToString() + ", " + totalTime.Ticks);
                }


                if (state.IsKeyDown(Keys.S) & !previousState.IsKeyDown(Keys.S)) // & state.IsKeyDown(Keys.LeftAlt)
                {
                    countdown = true;
                    clockOnOff = true;
                    startTime = DateTime.Now.Add(TimeSpan.Parse(counterduration));
                    //startClock();
                    Xkeys.BLOn(6);

                }

                if (state.IsKeyDown(Keys.G) & !previousState.IsKeyDown(Keys.G)) // & state.IsKeyDown(Keys.LeftAlt)
                {
                    //loadState();
                }

                if (!state.IsKeyDown(Keys.LeftControl) & state.IsKeyDown(Keys.R) & !previousState.IsKeyDown(Keys.R)) // & state.IsKeyDown(Keys.LeftAlt)
                {
                    countdown = false;
                    clockColor = Color.White;
                    clockOnOff = false;
                    resetClock();
                }

                if (state.IsKeyDown(Keys.LeftControl) & state.IsKeyDown(Keys.A) & !previousState.IsKeyDown(Keys.A)) // & state.IsKeyDown(XnaKeys.LeftAlt)
                {

                    Load d = new Load();               
                   //d.CreateControl();
                    d.ShowDialog();
                    //d.Show();

                    MessageBoxK m = new MessageBoxK();
                    m.message = d.opgave;
                    m.ShowDialog();
                    if (d.opgave != null)
                    {
                        timeParse(d.opgave);
                    }
                    d.Dispose();
                }


                if (countdown == true)
                {
                    ploo = startTime.Subtract(DateTime.Now);
                    if (ploo.Ticks <= 0) { ploo = TimeSpan.Parse("0"); clockColor = Color.Red; Xkeys.BLOff(5); }

                    if ((ploo.Minutes * 60 + ploo.Seconds) <= 10)
                    {

                        if (ploo.Milliseconds > 500) clockColor = Color.White;
                        else clockColor = Color.Red;
                    }
                    //else clockColor = Color.Black;
                }
                else
                {
                    startTime = DateTime.Now;
                }
            }
            cl0 = clots[0].ToString();
            base.Update(gameTime);
            previousState = state;
        }

        private void resetClock()
        {
            //throw new NotImplementedException();
            ploo = TimeSpan.Parse(counterduration);
            Xkeys.BLOff(6);
        }

        private void startClock()
        {
            //throw new NotImplementedException();
        }

        private void loadState()
        {
            inload = true;
            Debug.WriteLine("Load last state");
            string last = File.ReadLines("log.txt").Last();
            timeParse(last);
        }

        private void timeParse(string timeStr)
        {
            string[] last = timeStr.Split(',');
            totalTime = TimeSpan.Parse(last[2*numCand+1]);//"00:00:00:01");
            for (int i = 0; i < numCand; i++)
            {
                count[i] = false;
                swTotals[i].ElapsedTimeSpan = TimeSpan.Parse(last[i + 1]);// TimeSpan.Parse("00:00:00:00");
                                                                          // if (totalTime.Ticks > 0) percs[i] = Math.Round((((clots[i].Ticks) * 1000) / ((totalTime.Ticks))) / 10f).ToString() + "%";
                if (totalTime.TotalMilliseconds > 0) percs[i] = Math.Round(((((swTotals[i]).ElapsedTimeSpan.TotalMilliseconds) * 1000) / ((totalTime.TotalMilliseconds))) / 10f).ToString() + "%";

                else percs[i] = "0%";
                Debug.WriteLine(swTotals[i] + ", " + percs[i]);
            }
            for (int i = 0; i < numCand; i++)
            {
                swPartTotals[i].ElapsedTimeSpan = TimeSpan.Parse(last[i + numCand + 1]);// TimeSpan.Parse("00:00:00:00");+
                //clots[i] = clotsTotal[i];
                //if (totalTime.Ticks > 0) percs[i] = Math.Round((((clots[i].Ticks) * 1000) / ((totalTime.Ticks))) / 10f).ToString() + "%";
                Debug.WriteLine(swPartTotals[i] + ", " + percs[i]);
            }
            for (int i = 0; i < numCand; i++)
            {
                if (count[i])
                {
                    //clotsLast[i] = DateTime.Now;
                }
                else
                {
                    //clotsTotal[i] = clots[i];

                }
                //count[i] = true;
                //Thread.Sleep(40);
                //count[i] = false;
            }
            inload = false;
        }
        private void Load_ScreenRes()
        {
            Debug.WriteLine("Load Screen Resolution");
            int j = 0;
            string[] lines = new string[5]; ;
            foreach (string line in File.ReadLines("ScreenRes.txt")) lines[j++] = line;
            string[] res = lines[0].Split(',');
            wd = int.Parse(res[0]);
            ht = int.Parse(res[1]);
        }
        private void loadTotals()
        {
            Debug.WriteLine("Load Totals");
            int j = 0;
            string[] lines = new string[5]; ;
            foreach (string line in File.ReadLines("totals.txt")) lines[j++] = line;
            string[] partTotals = lines[0].Split(',');
            string[] totals = lines[1].Split(',');
            totalTime = TimeSpan.Parse("00:00:00");
            for (int i = 0; i < numCand; i++)
            {
                count[i] = false;
                swPartTotals[i] = new StopWatchWithOffset(TimeSpan.Parse("00:" + partTotals[i]));// TimeSpan.Parse("00:00:00:00");
                //totalTime = totalTime.Add(clots[i]);
            }
            for (int i = 0; i < numCand; i++)
            {

                if (totalTime.TotalMilliseconds > 0) percs[i] = Math.Round(((((swTotals[i]).ElapsedTimeSpan.TotalMilliseconds) * 1000) / ((totalTime.TotalMilliseconds))) / 10f).ToString() + "%";

                //Debug.WriteLine(clotsTotal[i] + ", " + percs[i]);

            }

        }


        private void saveState()
        {

            Debug.WriteLine("Save last state");
            using (StreamWriter w = File.AppendText("log.txt"))
            {
                string thing = DateTime.Now + ", ";
                for (int i = 0; i < numCand; i++)
                { thing += swTotals[i].ElapsedTimeSpan.ToString() + ", "; }
                for (int i = 0; i < numCand; i++)
                { thing += swPartTotals[i].ElapsedTimeSpan.ToString() + ", "; }
                thing += totalTime.ToString() + "\r\n";
                w.Write(thing);

                //    Log("Test1", w);
                //    Log("Test2", w);
            }
            Debug.WriteLine(File.ReadLines("log.txt").Last());
            //using (StreamReader r = File.OpenText("log.txt"))
            //{
            //    r.
            //    //DumpLog(r);
            //}
        }
        public static void DumpLog(StreamReader r)
        {
            string line;
            while ((line = r.ReadLine()) != null)
            {
                Debug.WriteLine(line);
            }
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            if (logoOnOff)
            {
                spriteBatch.Draw(itvlogo, new Rectangle(30, 30, 150, 150 * 380 / 576), Color.White * 0.5f);
                spriteBatch.Draw(mugshot, new Rectangle(wd - 135, 10, 125, 125), Color.White * 1.0f);
            }
            // Debug.WriteLine(startTime.Subtract(ploo));
            if(clockOnOff)
            spriteBatch.DrawString(font1, ploo.ToString(@"mm\:ss"),
                                   new Vector2(wd / 2 - (font1.MeasureString(startTime.Subtract(ploo).ToString(@"mm\:ss")).X / 2),
                                               ht / 3 - (font1.MeasureString(startTime.Subtract(ploo).ToString(@"mm\:ss")).Y / 2)), clockColor);


            for (int i = 0; i < numCand; i++)
            {

                spriteBatch.DrawString(font3, geezers[i], new Vector2((2 * i + 1) * wd / (2 * numCand) - font3.MeasureString(geezers[i]).X / 2, h + 30), Color.White);

            }

            for (int i = 0; i < numCand; i++)
            {
                spriteBatch.DrawString(font2, swPartTotals[i].ElapsedTimeSpan.ToString(@"mm\:ss"), new Vector2((2 * i + 1) * wd / (2 * numCand) - font2.MeasureString(swPartTotals[i].Elapsed.ToString(@"mm\:ss")).X / 2, h + 77), Color.White);
            }


            for (int i = 0; i < numCand; i++)
            {
                spriteBatch.DrawString(font2, percs[i], new Vector2((2 * i + 1) * wd / (2 * numCand) - font2.MeasureString(percs[i]).X / 2, h + 270), Color.White);
            }
  

            for (int i = 0; i < numCand; i++)
            {
                spriteBatch.DrawString(font2, swTotals[i].ElapsedTimeSpan.ToString(@"mm\:ss"), new Vector2((2 * i + 1) * wd / (2 * numCand) - font2.MeasureString(swPartTotals[i].Elapsed.ToString(@"mm\:ss")).X / 2, h + 170), Color.White);
            }
            draw_boxes();
            spriteBatch.End();
            base.Draw(gameTime);
        }

 
        protected void draw_boxes()
        {
            Primitives2D.DrawLine(spriteBatch, new Vector2(0, h), new Vector2(wd, h), Color.White, 5.0f);
            for (int i = 0; i <= numCand; i++)
            {

                Primitives2D.DrawLine(spriteBatch, new Vector2(i * wd / numCand, h), new Vector2(i * wd / numCand, ht), Color.White, 5.0f);

            }
        }
        public void print_state()
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine(clots[i].ToString() + ", " + clotsTotal[i].ToString());

            }
        }
        public static void Log(string logMessage, TextWriter w)
        {
            w.Write("\r\nLog Entry : ");
            w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString());
            w.WriteLine("  :");
            w.WriteLine("  :{0}", logMessage);
            w.WriteLine("-------------------------------");
        }
    }
    public class StopWatchWithOffset : Stopwatch
    {
        private Stopwatch _stopwatch = null;
        TimeSpan _offsetTimeSpan;

        public StopWatchWithOffset(TimeSpan offsetElapsedTimeSpan)
        {
            _offsetTimeSpan = offsetElapsedTimeSpan;
            _stopwatch = new Stopwatch();
        }

        public void Startw()
        {
            _stopwatch.Start();
        }

        public void Stopw()
        {
            _stopwatch.Stop();
        }

        public TimeSpan ElapsedTimeSpan
        {
            get
            {
                return _stopwatch.Elapsed + _offsetTimeSpan;
            }
            set
            {
                _offsetTimeSpan = value;
            }
        }
    }
}

