using System.Collections.Generic;
using UnityEngine;
using TextSpeech;
using UnityEngine.Android;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class SpeechTextManager : MonoBehaviour
{
    private string nombre;
    private string lastQuestion = "";
    private string lastanswer = "";
    public GameObject Character;
    private DateTime currentDateTime;
    public GameObject boton;
    [SerializeField] private string languaje = "es-ES";
    [SerializeField] private Text UiText;  

    [Serializable]
    public struct VoiceComand 
    {
        public string Keyword;
        public UnityEvent Response;
    }
    public VoiceComand[] voiceCommands;

    private Dictionary<string, UnityEvent> commands = new Dictionary<string, UnityEvent>(); 

    private void Awake()
    { 
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone)) 
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
#endif
        foreach (var command in voiceCommands) 
        {
            commands.Add(command.Keyword.ToLower(), command.Response);
        }
    }
    void Update()  
    { 
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            StopSpeaking("");
            StoptListening();
            Application.Quit();
        }
    }

    void Start()
    {
        currentDateTime = DateTime.Now;
        TextToSpeech.Instance.Setting(languaje, 1.2f, 1.2f);
        SpeechToText.Instance.Setting(languaje);

        SpeechToText.Instance.onResultCallback = OnFinalSpeechResult;
        TextToSpeech.Instance.onStartCallBack = OnSpeakStart;
        TextToSpeech.Instance.onDoneCallback = OnSpeakStop;
#if UNITY_ANDROID
        SpeechToText.Instance.onPartialResultsCallback = OnPartialSpeechResult;
#endif
        StartSpeaking("Hola, ¿cómo puedo ayudarte?");
    }

    //Speech to Text
    public void StartListening() 
    {
        SpeechToText.Instance.StartRecording();
    }
    public void StoptListening()
    {
        SpeechToText.Instance.StopRecording();
    }
    public void OnPartialSpeechResult(string result) 
    {
        UiText.text = result;
    }
    public void StartSpeaking(string message) 
    {
        lastanswer = message;
        TextToSpeech.Instance.StartSpeak(message);
    }
    public void StopSpeaking(string message)
    {
        TextToSpeech.Instance.StopSpeak();
    }
    public void OnSpeakStart() 
    {
        Debug.Log("Talking start...");
    }
    public void OnSpeakStop()
    {
        Debug.Log("Talking stop...");
    }
    public void Close() 
    {
        Application.Quit();
    }
    public void OnFinalSpeechResult(string result)
    {
        AutoComplete(result);
        UiText.text = result;
        if (result != null)
        {
            if (result.ToLower().Contains("mi nombre es") || result.ToLower().Contains("soy") || result.ToLower().Contains("hola mi nombre es")
                || result.ToLower().Contains("hola me llamo"))
            {
                // Extraer el nombre de la frase
                string[] palabras = result.Split(' ');
                nombre = palabras[palabras.Length - 1];

                // Responder con un mensaje de confirmación
                StartSpeaking("¡Mucho gusto, " + nombre + "! ¿En qué puedo ayudarte?");
            }
            else if (result.ToLower().Contains("cómo me llamo") || result.ToLower().Contains("cuál es mi nombre") || result.ToLower().Contains("dime mi nombre")
                || result.ToLower().Contains("mi nombre"))
            {
                if (!string.IsNullOrEmpty(nombre))
                {
                    // Responder con el nombre almacenado
                    StartSpeaking("Tu nombre es " + nombre);
                }
                else
                {
                    // Responder cuando no se ha proporcionado un nombre
                    StartSpeaking("Aún no me has dicho tu nombre. ¿Me lo puedes decir?");
                }
            }
            else if (result.ToLower().Contains("qué dijiste") || result.ToLower().Contains("Puedes repetir eso otra vez")
                || result.ToLower().Contains("repite eso") || result.ToLower().Contains("repite eso otra vez") || result.ToLower().Contains("repítelo"))
            {
                if (!string.IsNullOrEmpty(lastQuestion))
                {
                    // Repetir la última respuesta dada
                    StartSpeaking("lo ultimo que preguntaste fue" + lastQuestion + "y la respuesta fue" + lastanswer);
                }
                else
                {
                    // Generar una palabra al azar
                    var randomWord = GenerateRandomWord();

                    // Responder con la palabra generada al azar
                    StartSpeaking(randomWord);
                }
            }
            else
            {
                var response = commands.ContainsKey(result.ToLower()) ? commands[result.ToLower()] : null;
                if (response != null)
                {
                    response?.Invoke();
                }
                else
                {
                    // Generar una palabra al azar
                    var randomWord = GenerateRandomWord();

                    // Responder con la palabra generada al azar
                    StartSpeaking(randomWord);
                }
            }
        }
        // Almacenar la última respuesta dada
        lastQuestion = result;
    }
    private string GenerateRandomWord()
    {
        string[] words = {
            "Mis Disculpas, no logro captar las palabras lo que estás diciendo",
            "¿Podrías presionar el botón y repetir lo que dices, por favor?",
            "No me quedó claro lo que quisiste decir", 
            "No pude entender bien las palabras que dijiste",
            "¿Podrías hablar un poco más alto o cerca del micrófono, por favor?",
            "Creo que hubo un problema, ¿podrías presionar nuevamente el botón y hablar claramente?",  
            "No entendí ¿podrías repetirlo por favor?",
            "¿Podrías repetir esas palabras, por favor?",
            "No entendí lo que dijiste reciéntemente",
            "No logre escucharte muy bien, ¿podrías presionar nuevamente el botón y hablar claramente?",
            "Creo que no se logró captar todas las palabras",
            "¿Podrías repetir eso otra vez?"};
        var random = new System.Random();
        return words[random.Next(0, words.Length)];
    }

    private List<string> wordList = new List<string>() { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
    public void AutoComplete(string input)
    {
        string matchedWord = null;
        foreach (string word in wordList)
        {
            if (word.StartsWith(input))
            {
                matchedWord = word;
                break;
            }
        }
        if (matchedWord != null)
        {
            string autoComplete = matchedWord.Substring(input.Length);
            UiText.text = input + autoComplete;
        }
    }
    public void DondeEstas()
    {
        if (Character != null)
        {
            Vector3 newPosition = new Vector3(0, -2.79f, 15.46f);
            Quaternion newRotation = Quaternion.Euler(0, 180, 0);
            Vector3 newScale = new Vector3(-0.525965f, 0.525965f, 0.525965f);

            // Obtener la posición y rotación de la cámara del celular
            Vector3 cameraPosition = Camera.main.transform.position;
            Quaternion cameraRotation = Camera.main.transform.rotation;

            // Transformar la posición y rotación de la cámara al espacio de la pantalla del celular
            newPosition = cameraPosition + cameraRotation * newPosition;
            newRotation = cameraRotation * newRotation;

            // Asignar la posición, rotación y escala al personaje
            Character.transform.position = newPosition;
            Character.transform.rotation = newRotation;
            Character.transform.localScale = newScale;
        }
        else
        {
            Debug.LogError("El objeto del personaje no se encuentra activo en el proyecto.");
        }
    }
    public void SpeakCurrentDay()
    {
        string currentDay = "Hoy es " + DateTime.Now.ToString("dddd");
        StartSpeaking(currentDay);
    }
    public void SpeakCurrentDate()
    {
        string currentDate = "Hoy es " + DateTime.Now.ToString("dd/MM/yyyy");
        StartSpeaking(currentDate);
    }
    public void SpeakTomorrow()
    {
        string tomorrow = "Mañana es " + DateTime.Now.AddDays(1).ToString("dddd");
        StartSpeaking(tomorrow);
    }
    public void SpeakYesterday()
    {
        string yesterday = "Ayer fue " + DateTime.Now.AddDays(-1).ToString("dddd");
        StartSpeaking(yesterday);
    }

    public void SpeakTime()
    {
        currentDateTime = DateTime.Now;

        // convertir la hora a texto y dar el resultado en voz
        string currentTimeText = "La hora actual es " + currentDateTime.ToString("h:mm tt");
        StartSpeaking(currentTimeText);
        return;
    }
    public void SpeakMonth()
    {
        DateTime currentDateTime = DateTime.Now; 
         
        // Obtener el nombre del mes actual
        string currentMonth = currentDateTime.ToString("MMMM");

        // Combinar la información de la hora y el mes
        string fullTimeText = "estamos en el mes de " + currentMonth;

        // Dar el resultado en voz
        StartSpeaking(fullTimeText);
        return;
    }
    public void SpeakYear()
    {
        DateTime currentDateTime = DateTime.Now;
         

        // Obtener el año actual
        string currentYear = currentDateTime.ToString("yyyy");

        // Combinar la información del mes y el año
        string fullTimeText = "Estamos en el año " + currentYear;

        // Dar el resultado en voz
        StartSpeaking(fullTimeText);
        return;
    }


    private List<int> indicesUtilizados = new List<int>(); // Lista para almacenar los índices utilizados

    public void ObtenerDatoInteresante()
    {
        string[] datosInteresantes = {
        "El universo observable tiene aproximadamente noventa y tres mil millones de años luz de diámetro.",
        "El ADN humano es 99.9% idéntico entre todas las personas.",
        "Los pulpos tienen tres corazones.",
        "La luz tarda aproximadamente 8 minutos y 20 segundos en viajar desde el Sol hasta la Tierra.",
        "Los seres humanos comparten aproximadamente el 60% de su ADN con las bananas.",
        "El agua cubre alrededor del 71% de la superficie de la Tierra.",
        "El cerebro humano es el órgano más complejo del cuerpo y tiene alrededor de 86 mil millones de neuronas.",
        "Los tiburones han existido en la Tierra por más de 400 millones de años.",
        "El sonido viaja aproximadamente 343 metros por segundo en el aire.",
        "La Gran Muralla China tiene una longitud de alrededor de 21,196 kilómetros.",
        "El ojo humano puede distinguir aproximadamente 10 millones de colores.",
        "El corazón humano late alrededor de 100,000 veces al día.",
        "La Tierra gira a una velocidad de aproximadamente 1,670 kilómetros por hora en el ecuador.",
        "Los koalas no beben agua, obtienen la mayor parte de su hidratación de las hojas de eucalipto.",
        "La Torre Eiffel en París, Francia, tiene una altura de 324 metros.",
        "El hielo es menos denso que el agua, por eso flota en ella.",
        "La jirafa es el animal terrestre más alto, con una altura promedio de 5.5 metros.",
        "El Monte Everest, la montaña más alta del mundo, tiene una altura de 8,848 metros.",
        "El pulpo mímico puede cambiar de color y textura para camuflarse con su entorno.",
        "La Estatua de la Libertad en Nueva York, EE. UU., fue un regalo de Francia en 1886.",
        "El átomo es la unidad básica de la materia y está compuesto por protones, neutrones y electrones.",
        "El sistema solar se compone de ocho planetas: Mercurio, Venus, Tierra, Marte, Júpiter, Saturno, Urano y Neptuno.",
        "La Mona Lisa, pintada por Leonardo da Vinci, es una de las obras de arte más famosas del mundo.",
        "El cacao, el ingrediente principal del chocolate, se cultiva en países tropicales como Costa de Marfil, Ghana y Ecuador.",
        "El pingüino emperador es la especie de pingüino más grande y puede alcanzar una altura de hasta 1.2 metros.",
        "El sol es una estrella que se encuentra a unos 149.6 millones de kilómetros de la Tierra.",
        "El río Amazonas en América del Sur es el río más largo y caudaloso del mundo.",
        "El ajedrez es considerado uno de los juegos de estrategia más antiguos y desafiantes.",
        "El plátano es la fruta más consumida en el mundo." 
    };


        var random = new System.Random();
        int indice;

        if (indicesUtilizados.Count == datosInteresantes.Length)
        {
            // Si se han utilizado todos los datos interesantes, reiniciar la lista de índices utilizados
            indicesUtilizados.Clear();
        }

        do
        {
            indice = random.Next(0, datosInteresantes.Length);
        }
        while (indicesUtilizados.Contains(indice)); // Repetir hasta obtener un índice no utilizado

        indicesUtilizados.Add(indice); // Agregar el índice utilizado a la lista

        string datoInteresante = datosInteresantes[indice];

        StartSpeaking("Sabías que, " + datoInteresante);
    }
    public void DecirTrabalenguas()
    {
        List<string> trabalenguas = GenerarTrabalenguas();

        if (trabalenguas != null && trabalenguas.Count > 0)
        {
            var random = new System.Random();
            int index = random.Next(0, trabalenguas.Count);
            string trabalenguasToSpeak = trabalenguas[index];
            StartSpeaking(trabalenguasToSpeak);
        }
        else
        {
            StartSpeaking("Lo siento, no tengo ningún trabalenguas para decir en este momento.");
        }
    }

    public List<string> GenerarTrabalenguas()
    {
        List<string> trabalenguas = new List<string>()
    {
        "Tres tristes tigres tragan trigo en un trigal",
        "Pedro Pérez pintor perpetuamente pinta paredes",
        "Pablito clavó un clavito en la calva de un calvito",
        "El perro de San Roque no tiene rabo porque Ramón Ramírez se lo ha robado",
        "Como poco coco como, poco coco compro",
        "Si tu gusto gustara del gusto que gusta mi gusto, mi gusto gustaría del gusto que gusta tu gusto",
        "El cielo está enladrillado, ¿quién lo desenladrillará? El desenladrillador que lo desenladrille, buen desenladrillador será",
        "Pepe Peña pela papa, pica piña, pita un pito, apila pipas",
        "Tres tristes tigres tragan trigo en un trigal, en tres tristes trastos, tragaban trigo tres tristes tigres",
        "Juan tuvo un tubo, y el tubo que tuvo se le rompió, y para recuperar el tubo que tuvo, tuvo que comprar un tubo igual al tubo que tuvo"
    };

        return trabalenguas;
    }
    public void ContarHistoria()
    {
        List<string> historias = GenerarHistorias();

        if (historias != null && historias.Count > 0)
        {
            var random = new System.Random();
            int index = random.Next(0, historias.Count);
            string historiaToTell = historias[index];
            StartSpeaking(historiaToTell);
        }
        else
        {
            StartSpeaking("Lo siento, no tengo ninguna historia para contar en este momento.");
        }
    }

    public List<string> GenerarHistorias()
    {
        List<string> historias = new List<string>()
    {
        "Había una vez en un lejano reino, un valiente caballero que se embarcó en una peligrosa misión para rescatar a la princesa.",
        "En un pequeño pueblo, vivía un anciano sabio que poseía el secreto de la eterna felicidad.",
        "En lo profundo de la selva tropical, un explorador descubrió una antigua civilización perdida.",
        "En una noche estrellada, un niño soñador hizo un deseo que cambiaría su vida para siempre.",
        "En un planeta lejano, una raza alienígena luchaba por preservar la paz en su galaxia.",
        "En un tranquilo pueblo costero, una extraña criatura marina emergió de las profundidades del océano.",
        "En un mundo mágico, un aprendiz de mago se embarcó en un viaje para dominar sus poderes.",
        "En una tierra desolada, un grupo de supervivientes luchaba contra las adversidades para reconstruir su hogar.",
        "En el siglo XVIII, un intrépido pirata buscaba un legendario tesoro enterrado en una isla remota.",
        "En un futuro distópico, la humanidad se enfrentaba a un desafío existencial que cambiaría el curso de la historia."
    };

        return historias;
    }

    public void ContarChiste()
    {
        string[] chistes = {
        "¿Por qué los pájaros no usan Facebook?\nPorque ya tienen Twitter.",
        "¿Cómo se llama el campeón de buceo japonés?\nTokofondo.\n¿Y el subcampeón?\nKasitoko.",
        "Estás obsesionado con la comida.\nNo sé a qué te refieres croquetamente.",
        "¿Qué hace una abeja en el gimnasio?\n¡Zum-ba!",
        "¿Por qué estás hablando con esas zapatillas?\nPorque pone 'converse'.",
        "Mamá, mamá, en el colegio me llaman Facebook.\n¿Y tú qué les dices?\n¡Me gusta!",
        "¿Qué le dijo un semáforo a otro?\nNo me mires, me estoy cambiando.",
        "¿Cómo se llama el campeón de buceo japonés más rápido?\n¡Takahashi!",
        "Mamá, mamá, en el colegio me llaman WhatsApp.\n¿Y tú qué les dices?\n¡Espera, que estoy en línea!",
        "¿Qué hace un abogado en el océano?\n¡Nada, porque no se puede confiar en las olas!",
        "¿Por qué los peces no usan Facebook?\nPorque ya tienen su propio muro.",
        "Mamá, mamá, en el colegio me llaman WaiFai.\n¿Y tú qué les dices?\n¡Apártense, que voy sin cables!",
        "¿Por qué los perros no usan computadoras?\n¡Porque no les gustan los ratones!",
        "Mamá, mamá, en el colegio me llaman Instagram.\n¿Y tú qué les dices?\n¡Filtro, que eso no es cierto!",
        "¿Por qué los astronautas no pueden comer frijoles en el espacio?\n¡Porque los frijoles son flatulentos!",
        "Mamá, mamá, en el colegio me llaman Twitter.\n¿Y tú qué les dices?\n¡Sígueme, que no te vas a arrepentir!",
        "¿Por qué los tiburones no atacan a los abogados?\n¡Profesional cortado en el agua!",
        "Mamá, mamá, en el colegio me llaman Google.\n¿Y tú qué les dices?\n¡Búsquenlo ustedes mismos!"
    };

        var random = new System.Random();
        int index;

        if (indicesUtilizados.Count == chistes.Length)
        {
            // Si se han utilizado todos los chistes, reiniciar la lista de índices utilizados
            indicesUtilizados.Clear();
        }

        do
        {
            index = random.Next(0, chistes.Length);
        }
        while (indicesUtilizados.Contains(index)); // Repetir hasta obtener un índice no utilizado

        indicesUtilizados.Add(index); // Agregar el índice utilizado a la lista

        string chiste = chistes[index];

        StartSpeaking(chiste);
    }
    public void Bailar()
    {
        Character.GetComponent<Animator>().SetBool("Dance", true);
        StartSpeaking("Unnnnnnnnnn dossssss, Unnnnnnnnnnnnnnn dossssss, uuuunn doss, uuuuuuuunn dossss");
        Invoke("DetenerBailar", 5f);
    }

    private void DetenerBailar()
    {
        Character.GetComponent<Animator>().SetBool("Dance", false);
    }
    public void Mover()
    {
        Character.GetComponent<Animator>().SetBool("Move", true);
        StartSpeaking("moviéndome, moviéndome, tut, tut, tut, tut, uuuuuuuuuoooo, uoooooooooooooooooooooooooooooooowwwppp, a que no me encuentras");

        Invoke("DetenerMover", 3f);
    }

    private void DetenerMover()
    {
        Character.GetComponent<Animator>().SetBool("Move", false);
    }

    public void Girar()
    {
        Character.GetComponent<Animator>().SetBool("Girar", true);
        StartSpeaking("Girandooooooooooooooooooooooooooooooo, uooooooooooooooooooooouuhhh");

        Invoke("DetenerGirar", 3f);
    }

    private void DetenerGirar()
    {
        Character.GetComponent<Animator>().SetBool("Girar", false);
    }
    public void Alejar()
    {
        Character.GetComponent<Animator>().SetBool("Alejar", true);
        StartSpeaking("Aqui es suficiente?");

    }
    public void Acercar()
    {
        Character.GetComponent<Animator>().SetBool("Alejar", false);
        StartSpeaking("Listo, me he acercado");
    }
    public void Cantar()
    {
        StartSpeaking("La la la, estoy cantando... a la la la la long, a la la la la long long li lon lon lon, ou nou");
    }
    public void Detener() 
    {
        Character.GetComponent<Animator>().SetBool("Alejar", false);
        Character.GetComponent<Animator>().SetBool("Girar", false);
        Character.GetComponent<Animator>().SetBool("Move", false);
        Character.GetComponent<Animator>().SetBool("Dance", false);
    }
}
