using Jypeli;
using Jypeli.Assets;
using System;
using System.Collections.Generic;

public class ht2 : PhysicsGame
{
    Image HahmonKuva = LoadImage("hahmo");
    Image ZombienKuva = LoadImage("zombi");
    Image MaanKuva = LoadImage("maa");
    Image seinanKuva = LoadImage("seina");
    AssaultRifle rynkky;
    Pelaaja pelaaja;
    LaserGun laser;
    IntMeter pisteLaskuri;

    Boolean vihollisiaJaljella = false;
    int vihollisteMaara;
    int pelaajanTerveys = 10;
    int kierros = 0;
    private static String[] mappi =
    {
        "#---------------------------#",
        "#...........................#",
        "#...........................#",
        "#...........................#",
        "#...........................#",
        "#...........................#",
        "#...........................#",
        "#...........................#",
        "#...........................#",
        "#...........................#",
        "#...........................#",
        "#---------------------------#",
    };
    private static int tileWidth = 1980 / mappi[0].Length;
	private static int tileHeight = 1000 / mappi.Length;

    private TileMap ruudut = TileMap.FromStringArray(mappi);
    public override void Begin()
    {

       IsFullScreen = true;

        

        Valikko();


        // Kirjoita ohjelmakoodisi tähän
        
        

        

        Camera.ZoomToLevel();




        

        

    }
    void AloitaPeli()
    {
        


        ClearAll();
        
        

        ruudut.SetTileMethod('#', LuoSeina);
        ruudut.SetTileMethod('-', LuoKatto);
        ruudut.Execute(tileWidth, tileHeight);

        //rynkky
        rynkky = new AssaultRifle(30, 10);
        rynkky.Ammo.Value = 100;
        rynkky.ProjectileCollision = Osuma;
        //rynkky.ProjectileCollision = CollisionHandler.DestroyObject;
        //???? pitäs saada että ammukset osuvat vain zombiehin
       
        //laser
        laser = new LaserGun(30, 10);
        laser.Ammo.Value = 20;
        laser.ProjectileCollision = Osuma;



        //pelaaja
        pelaaja = new Pelaaja(10, 30);
        pelaaja.CanRotate = false;
        pelaaja.Elamat = 10;
        
        pelaaja.Image = HahmonKuva;
        pelaaja.Tag = "Pelaaja";
        Add(pelaaja);
        pelaaja.Add(rynkky);

        Level.Background.Image = MaanKuva;
        Level.Background.ScaleToLevelFull();

        //pistelaskuri peliin
        PisteLaskuri();

        //näppäimet

        Keyboard.Listen(Key.Down, ButtonState.Down, LiikutaPelaajaa, null, new Vector(0, -100), pelaaja);
        Keyboard.Listen(Key.Down, ButtonState.Pressed, KaannaAse, "Käännetään asetta pelaajanmukana", Angle.FromDegrees(-90), rynkky);
        Keyboard.Listen(Key.Up, ButtonState.Down, LiikutaPelaajaa, null, new Vector(0, 100), pelaaja);

        Keyboard.Listen(Key.Up, ButtonState.Pressed, KaannaAse, "Käännetään asetta pelaajanmukana", Angle.FromDegrees(90), rynkky);
        Keyboard.Listen(Key.Left, ButtonState.Down, LiikutaPelaajaa, null, new Vector(-100, 0), pelaaja);
        Keyboard.Listen(Key.Left, ButtonState.Pressed, KaannaAse, "Käännetään asetta pelaajanmukana", Angle.FromDegrees(180), rynkky);
        Keyboard.Listen(Key.Right, ButtonState.Down, LiikutaPelaajaa, null, new Vector(100, 0), pelaaja);
        Keyboard.Listen(Key.Right, ButtonState.Pressed, KaannaAse, "Käännetään asetta pelaajanmukana", Angle.FromDegrees(0), rynkky);
        Keyboard.Listen(Key.Space, ButtonState.Down, Ammu, "Ammu", rynkky, pelaaja);

        Keyboard.Listen(Key.Q, ButtonState.Pressed, AnnaLaser, "Annetaan laserGun", pelaaja);

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");

        
            Kierros();
        
        

    }
    //alkuvalikko
    void Valikko()
    {

        ClearAll();
        List<Label> painikkeet = new List<Label>();
        
        Label aloita = new Label("Start Game");
        aloita.Position = new Vector(0, 40);
        painikkeet.Add(aloita);
        aloita.BorderColor = Color.Black;
        foreach(Label painike in painikkeet)
        {
            Add(painike);
        }
        Mouse.ListenOn(aloita, MouseButton.Left, ButtonState.Pressed, AloitaPeli, null);
            
    }
    //lopeusnäyttö
    void Lopeta()
    {

        ClearAll();
        List<Label> painikkeet = new List<Label>();

        Label pisteet = new Label("pisteet: " + pisteLaskuri.Value);
        Label lopeta = new Label("Loppu");
        lopeta.Position = new Vector(0, 40);
        painikkeet.Add(lopeta);
        painikkeet.Add(pisteet);
        lopeta.BorderColor = Color.Black;
        foreach (Label painike in painikkeet)
        {
            Add(painike);
        }
        Mouse.ListenOn(lopeta, MouseButton.Left, ButtonState.Pressed, AloitaPeli, null);

    }
    //kierrosten ja vihujen luonti
    void Kierros()
    {
        kierros++;
        vihollisteMaara = 2 * kierros;
        //viholliset seuraajiksi

        //vihujenluonti
        for (int z = 0; z < vihollisteMaara; z++)
        {
            FollowerBrain VihunAivot = new FollowerBrain(pelaaja)
            {
                Speed = 10,
                DistanceFar = 900,
                DistanceClose = 10
            };

            Zombie zombi = new Zombie(10, 30);
            zombi.Image = ZombienKuva;
            zombi.Position = zombi.Respaus;
            zombi.Tag = "zombi";
            zombi.Brain = VihunAivot;
            zombi.CanRotate = false;

           AddCollisionHandler(pelaaja, zombi, Hyokkaa);


            Add(zombi);

        }
    }
 
    void LuoSeina(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject seina = PhysicsObject.CreateStaticObject(leveys, korkeus);
        seina.Position = paikka;
        seina.Shape = Shape.Rectangle;
        seina.Image = seinanKuva;
        seina.Color = Color.Black;
        seina.Tag = "seina";
        
        seina.CollisionIgnoreGroup = 1;
        
        Add(seina);
    }
    void LuoKatto(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject seina = PhysicsObject.CreateStaticObject(leveys, korkeus);
        seina.Position = paikka;
        seina.Shape = Shape.Rectangle;
        seina.Image = seinanKuva;
        seina.Color = Color.Black;
        seina.CollisionIgnoreGroup = 1;
        Add(seina);
    }
    //luodaan pistelaskuri
    void PisteLaskuri()
    {
        pisteLaskuri = new IntMeter(0);

        Label pisteNaytto = new Label();
        pisteNaytto.X = Screen.Left + 100;
        pisteNaytto.Y = Screen.Top - 100;
        pisteNaytto.TextColor = Color.Black;
        pisteNaytto.Color = Color.White;
        pisteNaytto.Title = "pisteet";
        pisteNaytto.BindTo(pisteLaskuri);
        Add(pisteNaytto);
    }

    class Pelaaja : PhysicsObject
    {
        public int Elamat { get; set; }
        public Vector Respaus { get; set; }
        public Color Vari { get; set; }
        public int Pisteet { get; set; }

        public Pelaaja(double leveys, double korkeus)
            : base(leveys, korkeus)
        {
            
            Elamat = 10;
            Pisteet = 0;
            Respaus = Vector.Zero;
            Color = Color.Beige;
        }
    }

    class Zombie : PhysicsObject
    {
        public int Elamat { get; set; }
        public Vector Respaus { get; set; }
        public Color Vari { get; set; }

        public Zombie(double leveys, double korkeus)
            : base(leveys, korkeus)
        {
            Elamat = 50;
            Respaus = RandomGen.NextVector(10, 10, 400, 400);
            Vari = Color.Red;
        }
    }

    class Piru : PhysicsObject
    {
        public int Elamat { get; set; }
        public Vector Respaus { get; set; }
        public Color Vari { get; set; }

        public Piru(double leveys, double korkeus)
            : base(leveys, korkeus)
        {
           
            Elamat = 300;
            
            Respaus = RandomGen.NextVector(0, 0, mappi[0].Length, mappi.Length);
            Vari = Color.Orange;
        }
    }

    
    //seinat ei sais tuhoutua
    void Osuma(PhysicsObject ammus, PhysicsObject kohde)
    {

        
        ammus.Destroy();
        if(kohde.Tag.Equals("zombi"))
        {
            pisteLaskuri.Value += 100;
            kohde.Destroy();
            vihollisteMaara--;
        }
        
        

        //miten vain että zombeihin osuu
        if(vihollisteMaara == 0)
        {
            
            Kierros();

        }
        
    }

    //liikutus
    void LiikutaPelaajaa(Vector suunta, Pelaaja pelaaja)
    {
        pelaaja.Push(suunta);
    }
    void Ammu(AssaultRifle rifle, Pelaaja pelaaja)
    {
        PhysicsObject ammus = rifle.Shoot();
        //ammus.Tag = "ammus";
        
        //AddCollisionHandler(ammus, "zombi", Osuma);
        //tämä kaataa pelin
        //ammus.CollisionIgnoreGroup = 1;

        if (ammus != null)
        {
           
        }
    }

    void KaannaAse(Angle aste, AssaultRifle rynkky)
    {
        rynkky.Angle = aste;
        
    }
   
    void Hyokkaa(Pelaaja pelaaja, PhysicsObject zombi)
    {
            kierros = 0;
            pelaaja.Destroy();
            Lopeta();
        
    }
    void AnnaLaser(Pelaaja pelaaja)
    {
        pelaaja.Add(laser);
    }
}
