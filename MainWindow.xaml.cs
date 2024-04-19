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

namespace PAC_Man_Game_WPF_MOD_ICT
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

        Rect pacmanHitbox;

        int ghostSpeed = 10;
        int ghostMoveStep = 160;
        int currentGhostStep;
        int score = 0;


        public MainWindow()
        {

            Console.WriteLine("Projeto do Jonathan");
            InitializeComponent();

            GameSetup();
        }

        private void CanvasKeyDown(object sender, KeyEventArgs e)
        {

            //Left
            if (e.Key == Key.Left && !noLeft)
            {
                goRight = goUp = goDown =false;
                noRight = noUp = noDown = false;

                goLeft = true;

                pacman.RenderTransform = new RotateTransform(-180, pacman.Width /2, pacman.Height / 2 );
            }
            // Right
            if ( e.Key == Key.Right && !noRight)
            {
                noLeft = noUp = noDown = false;
                goLeft = goUp = goDown = false;

                goRight = true;

                pacman.RenderTransform = new RotateTransform(0, pacman.Width / 2, pacman.Height / 2);
            }
            // Up
            if( e.Key == Key.Up && !noUp)
            {
                noRight = noLeft = noDown = false;
                goRight= goLeft = goDown = false;

                goUp = true;


                pacman.RenderTransform = new RotateTransform(-90, pacman.Width / 2, pacman.Height / 2);
            }

            if ( e.Key == Key.Down && !noDown)
            {
                noRight = noLeft = noUp = false;
                goRight = goLeft = goUp = false;

                goDown = true;

                pacman.RenderTransform = new RotateTransform(90, pacman.Width / 2, pacman.Height / 2);


            }



        }

        private void GameSetup()
        {

            MyCanvas.Focus();

            gameTimer.Tick += GameLoop; 
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Start();
            currentGhostStep = ghostMoveStep;

            ImageBrush pacmanImage = new ImageBrush();
            pacmanImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/pacman.jpg"));
            pacman.Fill = pacmanImage;

            ImageBrush redGhost = new ImageBrush();
            redGhost.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/red.jpg"));
            redGuy.Fill = redGhost;

            ImageBrush orangeGhost = new ImageBrush();
            orangeGhost.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/orange.jpg"));
            orangeGuy.Fill = orangeGhost;


            ImageBrush pinkGhost = new ImageBrush();
            pinkGhost.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/pink.jpg"));
            pinkGuy.Fill = pinkGhost;


        }

        private void GameLoop(object sender, EventArgs e)
        {
            txtScore.Content = "Score: " + score;

            if (goRight)
            {
                Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) + speed);
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
            if(goDown && Canvas.GetTop(pacman) + 80 > Application.Current.MainWindow.Height)
            {
                noDown = true; 
                goDown = false;
            }
            if (goUp && Canvas.GetTop(pacman) < 1)
            {
                noUp = true;
                goUp = false;
            }
            if(goLeft && Canvas.GetLeft(pacman) -10 < 1) 
            {
                noLeft = true;
                goLeft = false;
            }

            if(goRight && Canvas.GetLeft(pacman) + 70 > Application.Current.MainWindow.Width)
            {
                noRight = true;
                goRight = false;
            }

            pacmanHitbox = new Rect(Canvas.GetLeft(pacman), Canvas.GetTop(pacman), pacman.Width, pacman.Height);


            foreach(var x in MyCanvas.Children.OfType<Rectangle>())
            {
                Rect hitBox = new Rect(Canvas.GetLeft(x),Canvas.GetTop(x), x.Width, x.Height );

                if((string)x.Tag == "wall")
                {
                    if (goLeft == true && pacmanHitbox.IntersectsWith(hitBox))
                    {
                        Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) + 10);
                        noLeft = true;
                        goLeft = false;
                    }
                    if (goRight == true && pacmanHitbox.IntersectsWith(hitBox))
                    {
                        Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) - 10);
                        noRight = true;
                        goRight = false;
                    }
                    if (goDown == true && pacmanHitbox.IntersectsWith(hitBox))
                    {
                        Canvas.SetTop(pacman, Canvas.GetTop(pacman) - 10);
                        noDown = true;
                        goDown = false;
                    }
                    if (goUp == true && pacmanHitbox.IntersectsWith(hitBox))
                    {
                        Canvas.SetTop(pacman, Canvas.GetTop(pacman) + 10);
                        noUp = true;
                        goUp = false;
                    }

                }

                if ((string) x.Tag == "coin") // essa tag é o que esta defino na construção da tela la MainWindow.xaml
                {
                    if(pacmanHitbox.IntersectsWith(hitBox) && x.Visibility == Visibility.Visible)
                    {
                        x.Visibility = Visibility.Hidden;
                        score++;

                    }
                }


                if((string) x.Tag == "ghost")
                {
                    if (pacmanHitbox.IntersectsWith(hitBox))
                    {
                        GameOver("Que Azar! O Fantasma te achou, selecione Ok para tentar novamente"); 
                    }
                    if (x.Name.ToString() == "orangeGuy")
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - ghostSpeed);
                    }
                    else
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + ghostSpeed);
                    }

                    currentGhostStep--;

                    if(currentGhostStep < 1) 
                    {
                        currentGhostStep = ghostMoveStep;
                        ghostSpeed = -ghostSpeed;
                    
                    }
                }
            }


            // 85 coins win the game 

            if (score == 85)
            {
                GameOver("Parabéns, voce pegou todas as moedas, Selecione Ok para jogar novamente");
            }


        }

        private void GameOver(string message)
        {
            gameTimer.Stop();
            MessageBox.Show(message, "The Pac Man Game MOD ICT foi encerrado");

            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }


        
    }
}
