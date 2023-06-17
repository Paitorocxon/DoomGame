using System;
using System.Drawing;
using System.Windows.Forms;

public class Form1 : Form
{
    private const int screenWidth = 800;
    private const int screenHeight = 600;
    private const float playerMoveSpeed = 0.1f;
    private const float playerRotationSpeed = 0.05f;

    private Bitmap screenBuffer;
    private Graphics screenGraphics;

    private float playerX = 2.5f;
    private float playerY = 2.5f;
    private float playerAngle = 0f;

    private bool[] keyStates;

    private Bitmap wallTexture;

    public Form1()
    {
        InitializeComponents();
        InitializeGame();
    }

    private void InitializeComponents()
    {
        this.Text = "Doom Game";
        this.ClientSize = new Size(screenWidth, screenHeight);
        this.KeyDown += DoomGame_KeyDown;
        this.KeyUp += DoomGame_KeyUp;
        this.Paint += DoomGame_Paint;
    }

    private void InitializeGame()
    {
        screenBuffer = new Bitmap(screenWidth, screenHeight);
        screenGraphics = Graphics.FromImage(screenBuffer);

        keyStates = new bool[256];
        for (int i = 0; i < keyStates.Length; i++)
        {
            keyStates[i] = false;
        }

        wallTexture = new Bitmap("wall_texture.png"); // Pfad zur gewünschten Textur-PNG-Datei

        this.Focus();
        this.DoubleBuffered = true;
    }

    private void DoomGame_KeyDown(object sender, KeyEventArgs e)
    {
        keyStates[(int)e.KeyCode] = true;
    }

    private void DoomGame_KeyUp(object sender, KeyEventArgs e)
    {
        keyStates[(int)e.KeyCode] = false;
    }

    private void DoomGame_Paint(object sender, PaintEventArgs e)
    {
        screenGraphics.Clear(Color.Black);

        // Rendern der 3D-Umgebung
        for (int x = 0; x < screenWidth; x++)
        {
            // Berechnung des Abstands zur Wand
            float rayAngle = (playerAngle - 0.3f) + ((float)x / screenWidth) * 0.6f;
            float rayX = (float)Math.Sin(rayAngle);
            float rayY = (float)Math.Cos(rayAngle);
            float distanceToWall = 0f;
            bool hitWall = false;

            while (!hitWall && distanceToWall < 20f)
            {
                distanceToWall += 0.1f;
                int testX = (int)(playerX + rayX * distanceToWall);
                int testY = (int)(playerY + rayY * distanceToWall);

                // Kollisionsabfrage
                if (testX < 0 || testX >= 5 || testY < 0 || testY >= 5)
                {
                    hitWall = true;
                }
            }

            // Berechnung der Wandhöhe
            int ceiling = (int)(screenHeight / 2f - screenHeight / distanceToWall);
            int floor = screenHeight - ceiling;
            int wallHeight = floor - ceiling;

            // Rendern der Wände mit Textur
            int textureX = (int)(distanceToWall * 10) % wallTexture.Width; // Horizontale Texturkoordinate basierend auf der Entfernung zur Wand

            for (int y = ceiling; y < floor; y++)
            {
                int textureY = (y - screenHeight / 2 + wallHeight / 2) * wallTexture.Height / wallHeight; // Vertikale Texturkoordinate basierend auf der Wandhöhe
                Color wallColor = wallTexture.GetPixel(textureX, textureY);
                screenBuffer.SetPixel(x, y, wallColor);
            }
        }

        // Rendern des Spielers
        int playerSize = 3;
        int playerScreenX = (int)((screenWidth - playerSize) / 2f);
        int playerScreenY = (int)((screenHeight - playerSize) / 2f);
        screenGraphics.FillEllipse(Brushes.Red, playerScreenX, playerScreenY, playerSize, playerSize);

        // Zeichnen des Pufferbilds auf das Formular
        e.Graphics.DrawImage(screenBuffer, 0, 0);
    }

    private void UpdateGame()
    {
        // Spielerbewegung
        if (keyStates[(int)Keys.W])
        {
            MovePlayer(playerMoveSpeed);
        }
        if (keyStates[(int)Keys.S])
        {
            MovePlayer(-playerMoveSpeed);
        }
        if (keyStates[(int)Keys.A])
        {
            RotatePlayer(-playerRotationSpeed);
        }
        if (keyStates[(int)Keys.D])
        {
            RotatePlayer(playerRotationSpeed);
        }

        this.Invalidate();
    }

    private void MovePlayer(float distance)
    {
        float newX = playerX + (float)Math.Sin(playerAngle) * distance;
        float newY = playerY + (float)Math.Cos(playerAngle) * distance;

        // Kollisionsabfrage
        if (newX >= 0 && newX < 5 && newY >= 0 && newY < 5)
        {
            playerX = newX;
            playerY = newY;
        }
    }

    private void RotatePlayer(float angle)
    {
        playerAngle += angle;
        if (playerAngle < 0)
        {
            playerAngle += (float)(2 * Math.PI);
        }
        if (playerAngle >= 2 * Math.PI)
        {
            playerAngle -= (float)(2 * Math.PI);
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        screenGraphics.Dispose();
        screenBuffer.Dispose();
        wallTexture.Dispose();
        base.OnClosed(e);
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        // Hintergrund nicht zeichnen, um Flackern zu vermeiden
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        // Spiel aktualisieren
        UpdateGame();

        base.OnPaint(e);
    }


}
