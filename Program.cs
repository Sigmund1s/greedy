using Raylib_cs;
using System.Numerics;

// This is the parent class for the Circle and Rects classes and is responsible for initializing the x and y coordinates.
public class Object{
    protected static int x = 0;
    protected static int y = 0;
}

// This is the parent class of the Gems class and is responsible for the random x coordinate generator and setting setting up a Rectangle object.
public class Rects : Object{
    public static int setX(){
        Random rand = new Random();
        Rects.x = rand.Next(20, 780);
        return Rects.x;
    }
    public static Rectangle theRects(){
        return new Rectangle(setX(), Rects.y, 20, 20);
    }
}

// This is the Gems class which is responsible for the assigning the random color and assigning points to the colors.
public class Gems : Rects{
    private List<Color> theColors = new List<Color>(){Color.ORANGE, Color.GREEN, Color.YELLOW, Color.PINK, Color.BLUE, Color.RED, Color.GOLD};
    public static Color getColor(){
        Gems g = new Gems();
        Random rand = new Random();
        return g.theColors[rand.Next(0, g.theColors.Count)];
    }
    public static int getPoints(Color col){
        Gems g = new Gems();
        int points = 10;
        foreach(Color aColor in g.theColors){
            if (aColor.Equals(col)){
                return points;
            }
            else points+=10;
        }
        return 0;
    }
}

// This is the parent class of Player and Rock and sets up theVector, setVector, and getVector.
public class Circles : Object{
    public static int setX(){
        Random rand = new Random();
        Circles.x = rand.Next(20, 780);
        return Circles.x;
    }
    protected static Vector2 theVector = new Vector2(setX(), Circles.y);

    public void setVector(){
        theVector = new Vector2(setX(), Circles.y);
    }

    public static Vector2 getVector(){
        return theVector;
    }

}

// This is the Rock class where the points are initialized and the color.
public class Rock : Circles{
    public static Color getColor(){
        return Color.GRAY;
    }
    public static int getPoints(){
        return -50;
    }
}

// This is the player class that helps update the players position.
public class Player : Circles{
    new protected static int x = 400;
    new protected static int y = 400;
    public static Color getColor(){
        return Color.WHITE;
    }
    new protected static Vector2 theVector = new Vector2(Player.x, Player.y);
    new public static Vector2 getVector(){
        return theVector;
    }
    public void setTheVector(int velocityX, int velocityY){
        theVector.X += velocityX;
        theVector.Y += velocityY;
    }

}

// This is where the points are kept track of.
public class Points{
    protected static int points = 0;
    public static void setPoints(int add){
        points += add;
    }
    public static int getPoints(){
        return points;
    }
}

class Program
{
    public static void Main()
    {
        // This is where the screen is initialized.
        Raylib.InitWindow(800, 480, "Greed");
        Raylib.SetTargetFPS(60);

        Player p = new Player();
        Circles c = new Circles();

        // Lists for the gems and replacement lists.
        List<Rectangle> theRects = new List<Rectangle>();
        List<Color> theCols = new List<Color>();
        theRects.Add(Gems.theRects());
        theCols.Add(Gems.getColor());
        theRects.Add(Gems.theRects());
        theCols.Add(Gems.getColor());
        List<Rectangle> replace = new List<Rectangle>();
        List<Color> replaceTheCols = new List<Color>();

        // Rock lists and replace list.
        List<Vector2> theCircs = new List<Vector2>();
        theCircs.Add(Circles.getVector());
        List<Vector2> replaceTheCircs = new List<Vector2>();

        // The counters are responsible for keeping track of the number of frames that have passed through.
        int GemCounter = 0;
        int RockCounter = 0;
        while (!Raylib.WindowShouldClose())
        {
            // Where the screen is drawn.
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.BLACK);
            Raylib.DrawText("Points: " + Points.getPoints(), 20, 20, 20, Color.WHITE);
            // Initializes the player circle.
            Raylib.DrawCircleV(Player.getVector(), 10, Color.WHITE);

            // This is for the player movement.
            if(Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT) && Player.getVector().X < 790)
                p.setTheVector(3, 0);
            if(Raylib.IsKeyDown(KeyboardKey.KEY_LEFT) && Player.getVector().X > 10)
                p.setTheVector(-3, 0);
            if(Raylib.IsKeyDown(KeyboardKey.KEY_UP) && Player.getVector().Y > 10)
                p.setTheVector(0, -3);
            if(Raylib.IsKeyDown(KeyboardKey.KEY_DOWN) && Player.getVector().Y < 470)
                p.setTheVector(0,3);

            // This is where new gems are created and their attributes are added to theRects list and theCols list.
            if((GemCounter % 75) == 0 && GemCounter != 0){
                theRects.Add(Gems.theRects());
                theCols.Add(Gems.getColor());
                GemCounter = 0;
            }
            // This is where new rocks are created and their vectors are added to theCircs list.
            if((RockCounter % 45) == 0 && RockCounter != 0){
                c.setVector();
                theCircs.Add(Rock.getVector());
                RockCounter = 0;
            }
            // This is where the rocks are outputted to the window.
            foreach(Vector2 v in theCircs){
                Raylib.DrawCircleV(v, 15, Rock.getColor());
            }
            // This is where the gems are outputted to the window.
            int count = 0;
            foreach(var aRect in theRects){
                Raylib.DrawRectangleLinesEx(aRect, 5, theCols[count]);
                count++;
            }
            count = 0;
            // This is where the collissions are checked for the rocks are checked and then the list is appended to the updated y coordinates. If the rocks go off screen they are deleted from the list.
            foreach(Vector2 v in theCircs){
                if(Raylib.CheckCollisionCircles(Player.getVector(), 10, v, 15)){
                    Points.setPoints(Rock.getPoints());
                }
                else if(v.Y < 500){
                    Vector2 replaceVect = v;
                    replaceVect.Y += 2;
                    replaceTheCircs.Add(replaceVect);
                }
            }
            theCircs.Clear();
            // This is where the list is changed to the updated list.
            foreach(Vector2 v in replaceTheCircs)
                theCircs.Add(v);
            replaceTheCircs.Clear();

            // This is where the collissions for the gems are checked and then the list is appended to the updated y coordinates. If the gems go off screen they are deleted from the list.
            foreach(Rectangle aRect in theRects){
                if(Raylib.CheckCollisionCircleRec(Player.getVector(), 10, aRect))
                    Points.setPoints(Gems.getPoints(theCols[count]));
                else if(aRect.y < 500){
                    Rectangle rect = aRect;
                    rect.y += 2;
                    replace.Add(rect);
                    replaceTheCols.Add(theCols[count]);
                }
                count++;
            }
            theRects.Clear();
            theCols.Clear();
            count = 0;
            // This is where the lists are changed to the updated lists.
            foreach(Rectangle aRect in replace){
                theRects.Add(aRect);
                theCols.Add(replaceTheCols[count]);
                count++;
            }
            replace.Clear();
            replaceTheCols.Clear();

            GemCounter++;
            RockCounter++;
            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}