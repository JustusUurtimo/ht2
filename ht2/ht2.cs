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
    Image pirunKuva = LoadImage("piru");
    Image hpBoxi = LoadImage("Health");
    Image ammoBox = LoadImage("AmmoBox");

    AssaultRifle rynkky;
    public Pelaaja pelaaja1;
    public HpBox hp;
    public AmmoBox ammo;

    LaserGun laser;
    IntMeter pisteLaskuri;
    IntMeter hpLaskuri;
    List<Monsteri> vihut;
    int vihollisteMaara;
    
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

       //IsFullScreen = true;

        

        Valikko();


        // Kirjoita ohjelmakoodisi tähän
        
        

        

        Camera.ZoomToLevel();




        

        

    }
    void AloitaPeli()
    {
        


        ClearAll();
        
        

        ruudut.SetTileMethod('#', LuoSeina);
        ruudut.SetTileMethod('-', LuoSeina);
        ruudut.Execute(tileWidth, tileHeight);

        //pistelaskuri peliin

        PisteLaskuri();
        pisteLaskuri.Value = 30;

        
        //rynkky
        rynkky = new AssaultRifle(40, 30);
        rynkky.Ammo.Value = pisteLaskuri.Value;
        rynkky.ProjectileCollision = Osuma;
        
       
        //laser
        /*laser = new LaserGun(30, 10);
        laser.Ammo.Value = 20;
        laser.ProjectileCollision = Osuma;*/



        //pelaaja
        pelaaja1 = new Pelaaja(40, 80);
        pelaaja1.CanRotate = false;
        

        HpLaskuri();
        hpLaskuri.Value = pelaaja1.Elamat;

        pelaaja1.Image = HahmonKuva;
        pelaaja1.Tag = "Pelaaja";
        Add(pelaaja1);
        pelaaja1.Add(rynkky);

        Level.Background.Image = MaanKuva;
        Level.Background.ScaleToLevelFull();

        

        //näppäimet

        Keyboard.Listen(Key.Down, ButtonState.Down, LiikutaPelaajaa, null, new Vector(0, -100), pelaaja1);
        Keyboard.Listen(Key.Down, ButtonState.Released, Pysayta, null, new Vector(0, -100));
        Keyboard.Listen(Key.Down, ButtonState.Pressed, KaannaAse, "Käännetään asetta pelaajanmukana", Angle.FromDegrees(-90), rynkky);

        Keyboard.Listen(Key.Up, ButtonState.Down, LiikutaPelaajaa, null, new Vector(0, 100), pelaaja1);
        Keyboard.Listen(Key.Up, ButtonState.Released, Pysayta, null, new Vector(0, 100));
        Keyboard.Listen(Key.Up, ButtonState.Pressed, KaannaAse, "Käännetään asetta pelaajanmukana", Angle.FromDegrees(90), rynkky);

        Keyboard.Listen(Key.Left, ButtonState.Down, LiikutaPelaajaa, null, new Vector(-100, 0), pelaaja1);
        Keyboard.Listen(Key.Left, ButtonState.Released, Pysayta, null, new Vector(-100, 0));
        Keyboard.Listen(Key.Left, ButtonState.Pressed, KaannaAse, "Käännetään asetta pelaajanmukana", Angle.FromDegrees(180), rynkky);

        Keyboard.Listen(Key.Right, ButtonState.Down, LiikutaPelaajaa, null, new Vector(100, 0), pelaaja1);
        Keyboard.Listen(Key.Right, ButtonState.Released, Pysayta, null, new Vector(100, 0));
        Keyboard.Listen(Key.Right, ButtonState.Pressed, KaannaAse, "Käännetään asetta pelaajanmukana", Angle.FromDegrees(0), rynkky);

        Keyboard.Listen(Key.Space, ButtonState.Down, Ammu, "Ammu", rynkky, pelaaja1);

        Keyboard.Listen(Key.Q, ButtonState.Pressed, AnnaLaser, "Annetaan laserGun", pelaaja1);

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");

        
            Kierros();
        
        

    }
    //alkuvalikko
    void Valikko()
    {

        ClearAll();
        //jos aloitetaan uusipeli pitää kierroksen nollaantua
        kierros = 0;

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
        Mouse.ListenOn(lopeta, MouseButton.Left, ButtonState.Pressed, Valikko, null);

    }
    //tällä kauppa
    /*void Pause()
    {
        
        List<Label> painikkeet = new List<Label>();

        Label kauppa = new Label("Kauppa:");
        Label ostaHp = new Label("Osta eläimiä");
        Label ostaAmmo = new Label("Osta Ammuksia(maksetaan elämillä)");
        
        kauppa.Position = new Vector(0, 40);
        ostaHp.Position = new Vector(0, 50);
        ostaAmmo.Position = new Vector(0, 60);
        painikkeet.Add(kauppa);
        painikkeet.Add(ostaHp);
        painikkeet.Add(ostaAmmo);
        ostaHp.BorderColor = Color.Black;
        ostaAmmo.BorderColor = Color.Black;
        
        foreach (Label painike in painikkeet)
        {
            Add(painike);
        }
        Mouse.ListenOn(ostaHp, MouseButton.Left, ButtonState.Pressed, pelaajaOstaaHp, null);
        Mouse.ListenOn(ostaAmmo, MouseButton.Left, ButtonState.Pressed, pelaajaOstaaAmmo, null);
    }*/
    //kierrosten ja vihujen luonti
    void Kierros()
    {
        //lita vihuista
        vihut = new List<Monsteri>();

        kierros++;
        vihollisteMaara = 2 * kierros;
        

        //vihujenluonti
        for (int z = 0; z < vihollisteMaara; z++)
        {
            //viholliset seuraajiksi
            Zombie zombi = new Zombie(40, 80);

            FollowerBrain VihunAivot = new FollowerBrain(pelaaja1)
            {
                Speed = zombi.nopeus,
                DistanceFar = 1900,
                DistanceClose = 10
            };

            
            zombi.Image = ZombienKuva;
            zombi.Position = zombi.Respaus;
            zombi.Tag = "vihu";
            
            zombi.Brain = VihunAivot;
            zombi.CanRotate = false;

            AddCollisionHandler(pelaaja1, zombi, Hyokkaa);

            vihut.Add(zombi);
            Add(zombi);

        }
        // piruja joka 3. rundi
        if(kierros % 3 == 0) 
        {
            vihollisteMaara++;
            Piru piru = new Piru(100, 100);

            FollowerBrain VihunAivot = new FollowerBrain(pelaaja1)
            {
                Speed = piru.nopeus,
                DistanceFar = 1900,
                DistanceClose = 30
            };
            piru.Image = pirunKuva;
            piru.Brain = VihunAivot;
            piru.Tag = "vihu";
            piru.Position = piru.Respaus;
            while (piru.Position.Distance(pelaaja1.Position) < 40)
            {
                piru.Position = piru.Respaus;
            }

            piru.CanRotate = false;
            AddCollisionHandler(pelaaja1, piru, Hyokkaa);
            vihut.Add(piru);
            Add(piru);
            
            
        }
        //HpBoxit peliin, joka kierros chance saada 20%
        Random hpChance = new Random();
        if(hpChance.Next(0, 10) >= 8)
        {
            
            hp = new HpBox(30, 30);
            hp.Image = hpBoxi;
            AddCollisionHandler(pelaaja1, hp, Keraa);
            if(hp.hpBoxiKentalla == false)
            {
                hp.hpBoxiKentalla = true;
                Add(hp);
                
            }
            
        }
        //kauppa joka 3. kierros
        if(kierros % 3 == 0)
        {

            //Pause();
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
    //hp laskurin luonti
    void HpLaskuri()
    {
        hpLaskuri = new IntMeter(0);

        Label pisteNaytto = new Label();
        pisteNaytto.X = Screen.Left + 100;
        pisteNaytto.Y = Screen.Top - 130;
        pisteNaytto.TextColor = Color.Black;
        pisteNaytto.Color = Color.White;
        pisteNaytto.Title = "Elamat";
        pisteNaytto.BindTo(hpLaskuri);
        Add(pisteNaytto);
    }

    public class Pelaaja : PhysicsObject
    {
        public int Elamat { get; set; }
        public Vector Respaus { get; set; }
        public Color Vari { get; set; }
        public int Pisteet { get; set; }

        public Pelaaja(double leveys, double korkeus)
            : base(leveys, korkeus)
        {
            
            Elamat = 100;
            Pisteet = 0;
            Respaus = Vector.Zero;
            
        }
    }
    public class Monsteri : PhysicsObject
    {
        public int Elamat { get; set; }
        public Vector Respaus { get; set; }
        public Color Vari { get; set; }
        public int nopeus { get; set; }

        public Monsteri(double leveys, double korkeus)
            : base(leveys, korkeus)
        {
            Elamat = 50;
            nopeus = 30;
           
            Respaus = RandomGen.NextVector(-450, -450, 400, 400);

        }
    }


    public class Zombie : Monsteri
    {
        
        public Zombie(double leveys, double korkeus)
            : base(leveys, korkeus)
        {
            Elamat = 50;
            nopeus = 30;

            
        }
    }
    

    public class Piru : Monsteri
    {

        public Piru(double leveys, double korkeus)
            : base(leveys, korkeus)
        {
           
            Elamat = 100;
            nopeus = 40;
            
            
        }
    }


    public class HpBox : PhysicsObject
    {
        public bool hpBoxiKentalla { get; set; }
        public HpBox(double leveys, double korkeus)
            : base  (leveys, korkeus)
        {
            hpBoxiKentalla = false;
        }
    }
    public class AmmoBox : PhysicsObject
    {
        public bool ammoBoxiKentalla { get; set; }
        

        public AmmoBox(double leveys, double korkeus)
            : base(leveys, korkeus)
        {
            ammoBoxiKentalla = false;
        }
    }


    //Hp Kerays
    void Keraa(PhysicsObject pelaaja, PhysicsObject kerattava)
    {
        kerattava.Destroy();
        //hp.hpBoxiKentalla = false;
        if(kerattava.Tag.Equals("Hp"))
        {
            pelaaja1.Elamat += 25;
        }
        if(kerattava.Tag.Equals("Ammo"))
        {
            rynkky.Ammo.Value += 20;
            pisteLaskuri.Value = rynkky.Ammo.Value;
        }
       
        hpLaskuri.Value = pelaaja1.Elamat;
    }
    
    void Osuma(PhysicsObject ammus, PhysicsObject kohde)
    {
        bool tuhotaankoAmmus = true;
        rynkky.Ammo.Value--;
        
        pisteLaskuri.Value = rynkky.Ammo.Value;
        if (kohde.Tag.Equals("vihu"))
        {
            Random ohi = new Random();
            int dodged = ohi.Next(1, 10);
            if(dodged > 6)
            {
                kohde.Tag = "dodged";
            } else
            {
                kohde.Tag = "osui";
                
                
                

                
                
            }

            int monsterinSijainti = 0;
            Monsteri jotain = vihut[monsterinSijainti];
            while(monsterinSijainti < vihut.Capacity)
            { 
                if (vihut[monsterinSijainti].Tag.Equals("dodged"))
                {
                    jotain = vihut[monsterinSijainti];
                    
                    break;
                }
                if (vihut[monsterinSijainti].Tag.Equals("osui"))
                {
                    jotain = vihut[monsterinSijainti];
                    
                    break;
                }

                monsterinSijainti++;
            }
            
            if (jotain.Tag.Equals("dodged"))
            {
                tuhotaankoAmmus = false;
            jotain.nopeus += 30;
            FollowerBrain VihunAivot = new FollowerBrain(pelaaja1)
            {
               Speed = jotain.nopeus,
               DistanceFar = 900,
               DistanceClose = 10
            };
                jotain.Brain = VihunAivot;

            }
            if (jotain.Tag.Equals("osui"))
            {
                
                jotain.Elamat -= 25;
                if(jotain.Elamat <= 0)
                {
                    vihollisteMaara--;
                    vihut.Remove(vihut[monsterinSijainti]);
                    jotain.Destroy();
                    rynkky.Ammo.Value += 10;
                    pisteLaskuri.Value = rynkky.Ammo.Value;

                    //vihun taposta ammobox
                    ammo = new AmmoBox(30, 30);
                    ammo.Tag = "Ammo";
                    ammo.Image = ammoBox;
                    
                    AddCollisionHandler(pelaaja1, ammo, Keraa);
                    if (ammo.ammoBoxiKentalla == false)
                    {
                        ammo.ammoBoxiKentalla = true;
                        ammo.Position = jotain.Position;
                        Add(ammo);
                        
                    }
                }
                
            }
            jotain.Tag = "vihu";
        }
        //pisteLaskuri.Value--;
        if(tuhotaankoAmmus)
        {
            ammus.Destroy();
        }
        

        
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


    void Pysayta(Vector suunta)
    {
        pelaaja1.StopAxial(suunta);
    }


    void Ammu(AssaultRifle rifle, Pelaaja pelaaja)
    {
        PhysicsObject ammus = rifle.Shoot();
    }


    void KaannaAse(Angle aste, AssaultRifle rynkky)
    {
        rynkky.Angle = aste;
    }
   

    void Hyokkaa(PhysicsObject pelaaja, PhysicsObject zombi)
    {
            
            pelaaja1.Elamat -= 25;
            hpLaskuri.Value = pelaaja1.Elamat;
            pelaaja1.Move(zombi.Velocity * 2);
            if(pelaaja1.Elamat <= 0)
            {
            this.pelaaja1.Destroy();
            Lopeta();
            }
    }


    void AnnaLaser(Pelaaja pelaaja)
    {
        pelaaja.Add(laser);
    }


    void pelaajaOstaaHp()
    {
        pelaaja1.Elamat += 25;
        hpLaskuri.Value = pelaaja1.Elamat;
        pisteLaskuri.Value -= 25;
        rynkky.Ammo.Value = pisteLaskuri.Value;

    }
    
    void pelaajaOstaaAmmo()
    {
        pelaaja1.Elamat -= 10;
        
        pisteLaskuri.Value += 10;
        rynkky.Ammo.Value = pisteLaskuri.Value;
    }

}
