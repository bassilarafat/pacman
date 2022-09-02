using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace pacMAn
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        bool goLeft, goRight, goUp, goDown;
        bool noLeft, noRight, noUp, noDown;
        int speed = 8;
        Rect pacmanHitBox;

        int ghostSpeed = 10;
        int ghostMoveStep=130;
        int currentGhostStep;
        int score = 0;


        public MainWindow()
        {
            InitializeComponent();
            GameSetUp();
        }

        //ROTATE PACMAN IMAGE 
        private void canvasKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Left && noLeft == false)
            {
                goRight = goUp = goDown = false;             // set rest of the directions boolean to false
                noRight = noUp = noDown=  false;            // set rest of the restriction boolean to false
                goLeft = true;

                pacman.RenderTransform = new RotateTransform(-180, pacman.Width / 2, pacman.Height / 2);   // rotate the pac man image to face left
            }

            if(e.Key==Key.Right && noRight == false)
            {
                goLeft = goUp = goDown = false;
                noLeft = noUp = noDown=false;
                goRight = true;

                pacman.RenderTransform = new RotateTransform(0, pacman.Width / 2, pacman.Height / 2);
            }

            if (e.Key == Key.Up && noUp == false)
            {
                goLeft = goRight = goDown = false;
                noLeft = noRight = noDown = false;
                goUp = true;

                pacman.RenderTransform = new RotateTransform(-90, pacman.Width / 2, pacman.Height / 2);
            }

            if (e.Key == Key.Down && noDown == false)
            {
                goLeft = goUp = goRight = false;
                noLeft = noUp = noRight = false;         
                goDown = true;

                pacman.RenderTransform = new RotateTransform(90, pacman.Width / 2, pacman.Height / 2);
            }


        }


        private void GameSetUp()
        {

            // this function will run when the program loads

            myCanvas.Focus();                                                 // set my canvas as the main focus for the program

            gameTimer.Tick += GameLoop;                                     // link the game loop event to the time tick
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);            // set time to tick every 20 milliseconds
            gameTimer.Start();
            currentGhostStep = ghostMoveStep;                            // set current ghost step to the ghost move step

            ImageBrush pacmanImage = new ImageBrush();
            pacmanImage.ImageSource = new BitmapImage(new Uri("E:/cs/pacMAn/images/pacman.jpg"));
            pacman.Fill = pacmanImage;

            ImageBrush redGuyImage = new ImageBrush();
            redGuyImage.ImageSource = new BitmapImage(new Uri("E:/cs/pacMAn/images/red.jpg"));
            redGuy.Fill = redGuyImage;

            ImageBrush orangeGuyImage = new ImageBrush();
            orangeGuyImage.ImageSource = new BitmapImage(new Uri("E:/cs/pacMAn/images/orange.jpg"));
            orangGuy.Fill = orangeGuyImage;

            ImageBrush pinkGuyImage = new ImageBrush();
            pinkGuyImage.ImageSource = new BitmapImage(new Uri("E:/cs/pacMAn/images/pink.jpg"));
            PinkGuy.Fill = pinkGuyImage;


        }

        private void GameLoop(object sender, EventArgs e)
        {

            // this is the game loop event, this event will control all of the movements, outcome, collision and score for the game
            //SHOW THE SCORE IN THE REAL TIME 

            txtScore.Content = "score : " + score;

            // start moving the character in the movement directions
            if (goRight)
            {
                Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) + speed);   // if go right boolean is true then move pac man to the
                                                                          // right direction by adding the speed to the left 
            }

            if (goLeft)
            {
                Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) - speed);
            }

            if (goUp)
            {
                Canvas.SetTop(pacman, Canvas.GetTop(pacman) - speed);
            }

            if (goDown)
            {
                Canvas.SetTop(pacman, Canvas.GetTop(pacman) + speed);
            }

            // restrict the movement

            if (goDown && Canvas.GetTop(pacman) + 80 > Application.Current.MainWindow.Height)  // if pac man is moving down the position of pac man is
                                                                                               // grater than the main window height then stop down movement
            {
                noDown = true;
                goDown = false;
            }

            if (goUp && Canvas.GetTop(pacman) < 1)              // is pac man is moving and position of pac man is less than 1 then stop up movement
            {
                noUp = true;
                goUp = false;
            }

            if (goLeft && Canvas.GetLeft(pacman) - 10 < 1)
            {
                noLeft = true;
                goLeft = false;
            }

            if (goRight && Canvas.GetLeft(pacman) + 70 > Application.Current.MainWindow.Width)
            {
                noRight = true;
                goRight = false;
            }

            pacmanHitBox = new Rect(Canvas.GetLeft(pacman), Canvas.GetTop(pacman), pacman.Width, pacman.Height); // asssign the pac man hit box
                                                                                                                 // to the pac man rectangle (position)
            // below is the main game loop that will scan through all of the rectangles available inside of the game

            foreach (var x in myCanvas.Children.OfType<Rectangle>())
            {
                // loop through all of the rectangles inside of the game and identify them using the x variable
                Rect hitBox = new Rect(Canvas.GetLeft(x) , Canvas.GetTop(x) , x.Width, x.Height);     // create a new rect called hit box for all of the
                                                                                                      // available rectangles inside of the game
                // find the walls, if any of the rectangles inside of the game has the tag wall inside of it
                if ((string)x.Tag == "wall")
                {
                    // check if we are colliding with the wall while moving left if true then stop the pac man movement
                    if (goLeft == true && pacmanHitBox.IntersectsWith(hitBox))
                    {
                        Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) + 10);
                        noLeft = true;
                        goLeft = false;
                    }

                    // check if we are colliding with the wall while moving right if true then stop the pac man movement
                    if (goRight == true && pacmanHitBox.IntersectsWith(hitBox))
                    {
                        Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) - 10);
                        noRight = true;
                        goRight = false;
                    }
                    // check if we are colliding with the wall while moving down if true then stop the pac man movement
                    if (goDown == true && pacmanHitBox.IntersectsWith(hitBox))
                    {
                        Canvas.SetTop(pacman, Canvas.GetTop(pacman) - 10);
                        noDown = true;
                        goDown = false;
                    }
                    // check if we are colliding with the wall while moving up if true then stop the pac man movement
                    if (goUp == true && pacmanHitBox.IntersectsWith(hitBox))
                    {
                        Canvas.SetTop(pacman, Canvas.GetTop(pacman) + 10);
                        noUp = true;
                        goUp = false;
                    }

                }

                // check if the any of the rectangles has a coin tag inside of them

                if ((String)x.Tag == "coin")
                {
                    // if pac man collides with any of the coin and coin is still visible to the screen
                    if(pacmanHitBox.IntersectsWith(hitBox) && x.Visibility == Visibility.Visible)
                    {
                        // set the coin visiblity to hidden
                        x.Visibility = Visibility.Hidden;
                        // add 1 to the score
                        score++;
                    }
                }
                // if any rectangle has the tag ghost inside of it
                if ((string)x.Tag == "ghost")
                {
                    // check if pac man collides with the ghost
                    if (pacmanHitBox.IntersectsWith(hitBox))
                    {
                        // if collision has happened, then end the game by calling the game over function and passing in the message
                        GameEnd("Ghosts got you, click ok to play again");
                    }

                    // if there is a rectangle called orange guy in the game
                    if (x.Name.ToString() == "orangGuy")
                    {
                        // move that rectangle to towards the left of the screen
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - ghostSpeed);

                    }
                    else
                    {
                        // other ones can move towards the right of the screen
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + ghostSpeed);
                    }

                    // reduce one from the current ghost step integer
                    currentGhostStep--;

                    // if the current ghost step integer goes below 1 
                    if (currentGhostStep < 1)
                    {
                        // reset the current ghost step to the ghost move step value
                        currentGhostStep = ghostMoveStep;
                        // reverse the ghost speed integer
                        ghostSpeed = -ghostSpeed;
                    }
                }
                // if the player collected 85 coins in the game

                if (score == 94)
                {
                    // show game over function with the you win message
                    GameEnd("You Win, you collected all of the coins");
                }

            }

        }

        private void GameEnd(String message)
        {
            gameTimer.Stop();
            MessageBox.Show(message);

            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location); //reload the game
            Application.Current.Shutdown();

        }




    }
}
