namespace Memento;

/*  
Originator-luokka vastaa dokumentin sisällöstä.
Se voi myös luoda dokumentin tilasta uuden mementon,
tai ottaa vastaan ja palauttaa mementosta dokumentin aiemman tilan.
*/
class Originator
{    
    public string Teksti { get; private set; } = "";
    public int Versio { get; private set; } = 0;
 
    public void LisääRivi(string rivi) // Lisää olemassaolevaan tekstiin uuden tekstirivin ja päivittää versionumeron.
    {
        Teksti += $"\n{rivi}";
        Versio ++;
    }

    public IMemento TallennaTila() // Tallentaa dokumentin sen hetkisen tilan uuteen mementoon.
    {
        return new Memento(Teksti, Versio);
    }

    public void PalautaTila(IMemento _memento) // Palauttaa dokumentin aiemman tilan vastaanotetusta mementosta.
    {
        Memento memento = (Memento) _memento; // Vastaanotetun IMemento-olion tyyppimuunnos Mementoksi.
        Teksti = memento.Teksti;
        Versio = memento.Versio;
    }


    /*
    IMemento-rajapintaa toteuttava Memento-luokka on laitettu Originator-luokan sisään (nested class).
    Sen ollessa private ainoastaan Originator on tietoinen koko luokan olemassaolosta.
    Memento on muuttumaton (immutable),
    eli se saa tilansa luontihetkellä konstruktorilta eikä sitä voi muuttaa sen jälkeen.
    */
    private class Memento : IMemento
    {
        public string Teksti { get; } // Set puuttuu kokonaan, koska mementoa ei muuteta missään olosuhteissa.
        public int Versio { get; }

        public Memento(string teksti, int versio)
        {
            Teksti = teksti;
            Versio = versio;
        }
    }
}

/*
IMemento-rajapinta toimii "siltana" Originatorin ja Caretakerin välillä,
ja se jätetään ihan tarkoituksella tyhjäksi.
Kun mementoja käsitellään tämän rajapinnan kautta,
Caretaker ei saa tietää niiden tarkemmasta sisällöstä yhtään mitään.
*/
public interface IMemento { }


/*
Caretaker-luokka ottaa vastaan mementoja ja huolehtii niiden säilyttämisestä.
Mementoja arkistoidaan sekä Undo- että Redo-pinoihin,
joista niitä tarpeen mukaan palautetaan Originatorille.
*/
class Caretaker
{
    private Stack<IMemento> undoPino = new Stack<IMemento>();
    private Stack<IMemento> redoPino = new Stack<IMemento>();
    private readonly Originator originator; // Readonly estää Originatorin vaihtamisen myöhemmin.

    public Caretaker(Originator _originator)  // Caretaker sidotaan luontivaiheessa tiettyyn Originatoriin.
    {
        originator = _originator;
    }

    public void Tallenna() // Tallentaa dokumentin tilan Undo-pinoon.
    {
        undoPino.Push(originator.TallennaTila()); // Lisätään Originatorin luoma memento Undo-pinoon.
        redoPino.Clear(); // Redo-pino tyhjennetään, koska uuden rivin lisäys poistaa aiemmat Redo-mahdollisuudet.
    }

    public void Undo() // Kumoaa aiemman toiminnon.
    {
        if (undoPino.Count > 0){
            redoPino.Push(originator.TallennaTila()); // Ennen Undoa luodaan uusi memento ja lisätään Redo-pinoon.
            originator.PalautaTila(undoPino.Pop()); // Tila palautetaan Undo-pinon päällimmäisestä mementosta.
        }
    }

    public void Redo() // Tekee uudelleen aiemmin kumotun toiminnon.
    {
        if (redoPino.Count > 0){
            undoPino.Push(originator.TallennaTila()); // Ennen Redoa luodaan uusi memento ja lisätään Undo-pinoon.
            originator.PalautaTila(redoPino.Pop()); // Tila palautetaan Redo-pinon päällimmäisestä mementosta.
        }
    }
}


/*
Varsinainen pääohjelma näyttää dokumentin sisällön ja tarjoaa valikon dokumentin muokkaamiseen.
Valikkoon sisältyy toiminnot:
    1. Kirjoita (josta on tehty selkeyden vuoksi oma funktionsa)
    2. Undo (Caretakerin metodi)
    3. Redo (myös Caretakerin metodi)
    4. Lopeta
*/
public class Program
{
    public static void Main()
    {
        Originator originator = new Originator();
        Caretaker caretaker = new Caretaker(originator); // Caretaker-olio sidotaan heti juuri luotuun Originatoriin.

        while (true)
        {
            Console.Clear();
            Console.WriteLine($"\nDokumentti v{originator.Versio}");
            Console.WriteLine("-------------");
            Console.WriteLine(originator.Teksti);
            Console.WriteLine("\n-------------\n");
            Console.WriteLine("1. Kirjoita");
            Console.WriteLine("2. Undo");
            Console.WriteLine("3. Redo");
            Console.WriteLine("Lopeta (Enter)");

            string? valinta = Console.ReadLine(); // Syötteestä otetaan käyttäjän valinta talteen.

            if (valinta == "1")      // Valinta 1: kutsutaan Kirjoita-funktiota.
                Kirjoita(originator, caretaker);
            
            else if (valinta == "2") // Valinta 2: kutsutaan Caretakerin Undo-metodia.
                caretaker.Undo();
            
            else if (valinta == "3") // Valinta 3: kutsutaan Caretakerin Redo-metodia.
                caretaker.Redo();
            
            else if (valinta == "") // Enter eli tyhjä string katkaisee loopin ja ohjelma päättyy.
                break;
        }
    }

    /*
    Dokumenttiin kirjoittaminen on laitettu omaksi Kirjoita-funktiokseen.
    Kirjoitusprosessi pyörii loopissa ja käyttäjä voi halutessaan kirjoittaa
    dokumenttiin niin monta riviä tekstiä kuin haluaa yhden session aikana.

    Laukaiseva tekijä uuden mementon luomiseen on rivinvaihto. 
    Aina kun käyttäjä tekee rivinvaihdon Enteriä painamalla,
    dokumentista tehdään memento ja laitetaan Undo-pinoon mahdollista peruutusta varten.
    Toisin sanoen: kun käyttäjä on palannut Kirjoita-funktiosta takaisin valikkoon,
    hän voi perua (Undo) tai palauttaa (Redo) muutoksia yksi tekstirivi kerrallaan.

    Parametreinä otetaan sisään sekä Originator- että Caretaker-oliot,
    jotta niiden metodeja voidaan kutsua myös funktion sisältä.
    */
    internal static void Kirjoita(Originator originator, Caretaker caretaker)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"\nDokumentti v{originator.Versio}");
            Console.WriteLine("-------------");
            Console.WriteLine(originator.Teksti); // Dokumentin sisältö pidetään aina käyttäjän nähtävillä.
            
            string? rivi = Console.ReadLine(); // Syöte ottaa vastaan uuden tekstirivin.
                
            if (rivi == "") // Jos Enteriä painetaan heti rivin alussa, kirjoittaminen päättyy ja palataan valikkoon.
                return;
            
            caretaker.Tallenna(); // Dokumentin tila lisätään Caretakerin Undo-pinoon ENNEN uuden rivin lisäämistä.
            originator.LisääRivi(rivi!); // Dokumenttiin lisätään uusi tekstirivi Originatorin kautta.
        }           
    }
}        
