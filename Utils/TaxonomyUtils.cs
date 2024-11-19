using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;


namespace ECARules4All_DLL.Utils
{
    public class TaxonomyUtils
    {
        #if !UNITY_EDITOR && UNITY_WEBGL
            private static string _protocol = "http";
            private static string _domain = "localhost";
            private static string _port = "8080";
    
            private static string _streamingAssets = "StreamingAssets/";
        #else
            private static string _protocol = "file";
            private static string _domain = "";
            private static string _port = "";
        
            private static string _streamingAssets = Application.streamingAssetsPath + "/";
        #endif
    
        private static string _audiosFolder = _streamingAssets + "audios/";
        private static string _imagesFolder = _streamingAssets + "images/";
        private static string _videosFolder = _streamingAssets + "videos/";
        private static string _assetsFolder = _streamingAssets + "assets/";
        private static string _savesFolder = _streamingAssets + "saves/";
        private static string _ECAsystemFolder = _streamingAssets + "ECAsystem/";
    
        private static string _serverGetFolder = "getDirectoryFiles/"; // La get del server è mappata su questa pagina
    
        private static string jsonFile;
    
        #region AgileError(TM)
    
        public const string SUCCESS = "ok";
        public const string PARAMETERS_ERROR = "error-00";
        public const string OBJECT_NOT_FOUND = "error-01";
        public const string ILLEGAL_CHARACTERS = "error-02";
        public const string NAME_ALREADY_USED = "error-03";
        public const string PROPERTY_NOT_EXISTING = "error-04";
        public const string PROPERTY_CONFLICTING = "error-05";
        public const string ILLEGAL_NAME = "error-06";
        public const string CONNECTION_ERROR = "error-07";
        public const string CRITICAL_ERROR = "error-08";
        public const string INVALID_FORMAT = "error-09";
    
        public const string NOT_ECA_OBJECT = "error-eca-00";
        public const string ECA_RULE_NOT_VALID = "error-eca-01";
    
        public const string GENERIC_WARNING = "warn-00";
        public const string IS_COROUTINE = "warn-01";
        public const string SOURCE_WARNING = "warn-02";
    
        public const string LOG = "log";
    
        #endregion
    
        public const string AgileSep = "@";
    
        public static string JoinArgsWithAgileSep(params object[] args)
        {
            var newArgsAsString = string.Join(AgileSep, args);
            return newArgsAsString;
        }
    
        #region URLs getters
    
        /**
             * <summary> Restituisce l'indirizzo base del dominio.
             * Per WebGL, restituisce l'URL nel formato "protocollo://dominio:porta/".
             * 
             * Per le altre piattaforme il path completo fino a StreamingAssets.</summary>
             */
        public static string getBaseURL()
        {
            #if !UNITY_EDITOR && UNITY_WEBGL
                return _protocol + "://" + _domain + ":" + _port + "/";
            #else
                return _protocol + "://" + _domain + _port;
            #endif
    
            /*#if !UNITY_EDITOR && UNITY_WEBGL
                return _protocol + "://" + _domain + ":" + _port + "/";
            #else
                return "file://" + Path.Combine(Application.streamingAssetsPath);
            #endif*/
        }
    
        public static string getStreamingAssetsURL()
        {
            return getBaseURL() + _streamingAssets;
        }
    
        /**
             * <summary>Restituisce l'indirizzo della cartella 'audios'.
             * Per WebGL, restituisce l'URL nel formato <c>protocollo://dominio:porta/[...]/StreamingAssets/audios/</c>.
             * </summary>
             */
        public static string getAudiosFolderURL()
        {
            return getBaseURL() + _audiosFolder;
        }
    
        /**
             * <summary>Restituisce l'indirizzo del file audio richiesto.
             * <param name="file_name">Nome del file, compreso di estensione.</param>
             * 
             * Per WebGL, restituisce l'URL nel formato <c>protocollo://dominio:porta/[...]/StreamingAssets/audios/</c>.
             *
             * <returns>Path del file audio richiesto</returns>
             * </summary>
             */
        public static string getFileAudioByName(string file_name)
        {
            return getAudiosFolderURL() + file_name;
        }
    
        public static string getFileVideoByName(string file_name)
        {
            return getVideosFolderURL() + file_name;
        }
    
        public static string getFileImageByName(string file_name)
        {
            return getImagesFolderURL() + file_name;
        }
    
        public static string getFileAssetByName(string file_name)
        {
            return getAssetsFolderURL() + file_name;
        }
    
        public static string getFileSaveByName(string file_name)
        {
            return getSavesFolderURL() + file_name;
        }
    
        public static string getFileECAsystemByName(string file_name)
        {
            return getECAsystemFolderURL() + file_name;
        }
    
        private static string getImagesFolderURL()
        {
            return getBaseURL() + _imagesFolder;
        }
    
        private static string getVideosFolderURL()
        {
            return getBaseURL() + _videosFolder;
        }
    
        private static string getAssetsFolderURL()
        {
            return getBaseURL() + _assetsFolder;
        }
    
        private static string getSavesFolderURL()
        {
            return getBaseURL() + _savesFolder;
        }
    
        private static string getECAsystemFolderURL()
        {
            return getBaseURL() + _ECAsystemFolder;
        }
    
        private static string getFolderGetServerURL()
        {
            return _protocol + "://" + _domain + ":" + _port + "/" + _serverGetFolder;
        }
    
        #endregion
        
        /// <summary>
        /// Stampa un messaggio di errore esaustivo nella console. Utile per capire l'origine dei problemi su WebGL.
        /// </summary>
        /// <remarks>
        /// E' possibile usare dei codici per visualizzare log di errore (e), warning (w) o debug (l).
        /// <list type="bullet">
        ///     <item>(e) PARAMETERS_ERROR: "La funzione richiede funData parametri. Oggetto che l'ha richiesta: objName"</item>
        ///     <item>(e) OBJECT_NOT_FOUND: "L'oggetto chiamato objName non esiste"</item>
        ///     <item>(e) ILLEGAL_CHARACTERS: "Impossibile rinominare l'oggetto objName in funData. L'oggetto presenta dei caratteri illegali."</item>
        ///     <item>(e) NAME_ALREADY_USED: "Impossibile rinominare l'oggetto objName in funData. Nome già utilizzato."</item>
        ///     <item>(e) PROPERTY_NOT_EXISTING: "L'oggetto objName non dispone della proprietà funData"</item>
        ///     <item>(e) PROPERTY_CONFLICTING: "Impossibile applicare la proprietà funData.Split(AgileSep).First(). L'oggetto objName dispone della proprietà funData.Split(AgileSep).Last() che va in conflitto con quella richiesta."</item>
        ///     <item>(e) ILLEGAL_NAME: "Si è verificato un errore, riprova. Se il problema persiste, consulta le istruzioni del software con il seguente codice errore: error-08"</item>
        ///     <item>(e) CONNECTION_ERROR: "Si è verificato un problema nella richiesta HTTP per il file funData. Oggetto che ha richiesto la risorsa: objName"</item>
        ///     <item>(e) CRITICAL_ERROR: "Si è verificato un errore critico. Se il problema persiste, si consiglia di riavviare il software.\n Dettagli errore: funData"</item>
        ///     <item>(e) INVALID_FORMAT: "Formato della stringa non valido. Dettagli errore: funData"</item>
        ///     <item>(e) NOT_ECA_OBJECT: "Impossibile applicare la regola all'oggetto objName. L'oggetto non dispone di una proprietà per gestirla."</item>
        ///     <item>(e) ECA_RULE_NOT_VALID: "La regola non è valida. Oggetto che l'ha respinta: objName. Maggiori dettagli: funData"</item>
        ///     <item>(w) GENERIC_WARNING: "Warning generico. Dettagli: funData"</item>
        ///     <item>(w) IS_COROUTINE: "In attesa del termine della coroutine. Maggiori dettagli: funData" </item>
        ///     <item>(w) SOURCE_WARNING: "Il parametro source non può essere nullo o vuoto!"</item>
        ///     <item>(l) LOG: funData</item>
        /// </list>
        /// 
        /// </remarks>
        public static string AgileErrorStatus(string error, string funData, string objName,
            [CallerMemberName] string funName = null)
        {
            string agileError = "";
    
            switch (error)
            {
                case PARAMETERS_ERROR:
                {
                    agileError = error + ": (" + funName + "): La funzione richiede " + funData +
                                 " parametri. Oggetto che l'ha richiesta: " + objName;
                    Debug.LogError(agileError);
                    break;
                }
                case OBJECT_NOT_FOUND:
                {
                    agileError = error + ": (" + funName + "): L'oggetto chiamato '" + objName + "' non esiste.";
                    Debug.LogError(agileError);
                    break;
                }
                case ILLEGAL_CHARACTERS:
                {
                    agileError = error + ": (" + funName + "): Impossibile rinominare l'oggetto '" + objName + "' in " +
                                 funData + ". L'oggetto presenta dei caratteri illegali.";
                    Debug.LogError(agileError);
                    break;
                }
                case NAME_ALREADY_USED:
                {
                    agileError = error + ": (" + funName + "): Impossibile rinominare l'oggetto '" + objName + "' in " +
                                 funData + ". Nome già utilizzato.";
                    Debug.LogError(agileError);
                    break;
                }
                case PROPERTY_NOT_EXISTING:
                {
                    agileError = error + ": (" + funName + "): L'oggetto '" + objName + "' non dispone della proprietà " +
                                 funData;
                    Debug.LogError(agileError);
                    break;
                }
                case PROPERTY_CONFLICTING:
                {
                    agileError = error + ": (" + funName + "): Impossibile applicare la proprietà " +
                                 funData.Split(Convert.ToChar(AgileSep)).First() +
                                 ". L'oggetto '" + objName + "' dispone della proprietà " + funData.Split(Convert.ToChar(AgileSep)).Last() +
                                 ", che va in conflitto con quella richiesta.";
                    Debug.LogError(agileError);
                    break;
                }
                case ILLEGAL_NAME:
                {
                    // Errore volutamente vago per evitare attacchi per rompere il sistema. [Bart docet]
                    Debug.LogError(
                        "Si è verificato un errore, riprova. Se il problema persiste, consulta le istruzioni del software con il seguente codice errore: " +
                        error);
                    break;
                }
                case CONNECTION_ERROR:
                {
                    agileError = error + ": (" + funName +
                                 "): Si è verificato un problema nella richiesta HTTP per il file " + funData +
                                 ". Oggetto che ha richiesto la risorsa: " + objName;
                    Debug.LogError(agileError);
                    break;
                }
                case CRITICAL_ERROR:
                {
                    agileError = error + ": (" + funName +
                                 "): Si è verificato un errore critico. Se il problema persiste, si consiglia di riavviare il software.\n Dettagli errore: " +
                                 funData;
                    Debug.LogError(agileError);
                    break;
                }
                case INVALID_FORMAT:
                    agileError = error + ": (" + funName + "): Formato della stringa non valido. Dettagli errore: " + funData;
                    Debug.LogError(agileError);
                    break;
                case NOT_ECA_OBJECT:
                {
                    agileError = error + ": (" + funName + "): Impossibile applicare la regola all'oggetto '" + objName +
                                 "'. L'oggetto non dispone di una proprietà per gestirla.";
                    Debug.LogError(agileError);
                    break;
                }
                case ECA_RULE_NOT_VALID:
                {
                    agileError = error + ": (" + funName + "): La regola non è valida. Oggetto che l'ha respinta: " +
                                 objName + ". Maggiori dettagli: " + funData;
                    Debug.LogError(agileError);
                    break;
                }
                case IS_COROUTINE:
                {
                    agileError = error + ": (" + funName + "): In attesa del termine della coroutine. Maggiori dettagli: " +
                                 funData;
                    Debug.LogWarning(agileError);
                    break;
                }
                case GENERIC_WARNING:
                    agileError = error + ": (" + funName + "): Warning generico. Dettagli: " + funData;
                    Debug.LogWarning(agileError);
                    break;
                case SOURCE_WARNING:
                    agileError = error + ": (" + funName + "): Il parametro source non può essere nullo o vuoto! Dettagli: " + funData;
                    Debug.LogWarning(agileError);
                    break;
                case LOG:
                    agileError = error + ": (" + funName + " ): " + funData;
                    Debug.Log(agileError);
                    break;
            }
    
            return error;
        }
    
    
        public static bool CheckIfResourceExists(string resourceName, bool launchError=true)
        {
            var isValid =  Resources.Load(resourceName) != null;
            if (launchError)
            {
                if(!isValid)
                    throw new Exception($"[{resourceName}] is not valid Resource path.");
    
            }
            return isValid;
        }
    
        public static string TryRemovingFilenameExtension(string fileName)
        {
            return TryRemovingFilenameExtension(fileName, out bool hasBeenModified);
        }
        public static string TryRemovingFilenameExtension(string fileName, out bool hasBeenModified)
        {
            return TryRemovingFilenameExtension(fileName, out hasBeenModified,
                (oldString, newString) => $"Filename ({oldString}) contains a dot. Removing it. Result is {newString}");
        }
    
        public static string TryRemovingFilenameExtension(string fileName, out bool hasBeenModified, Func<string, string, string> customWarningCallback)
        {
            hasBeenModified = false;
            var output = fileName;
            
            if (fileName.Contains("."))
            {
                output = fileName.Substring(0, fileName.LastIndexOf('.'));
                Debug.LogWarning(customWarningCallback(fileName, output));
                hasBeenModified = true;
            }
            
            return output;
        }
    
        public static Material GetMaterialFromResourcesFolder(string materialName)
        {
            return Resources.Load(materialName) as Material;
        }
    
        public static Type GetTypeByString(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;
    
            var namespacesList = new List<string>() { "ECARules4All.RuleEngine", };
    
            foreach (var ns in namespacesList)
            {
                type = Type.GetType(ns + "." + typeName); // Per i namespace, bisogna cercare 'NameSpace, Tipo'
                if (type != null)
                    return type;
            }
    
            return null;
        }
    
        public static bool IsEcaObject(GameObject o)
        {
            return o.GetComponent<ECAObject>() != null;
        }
    
        public static string getJSONLocalhostUrlScene(string file_name)
        {
    #if !UNITY_EDITOR && UNITY_WEBGL
                return "http://localhost:8080/StreamingAssets/" + file_name;
    #else
            return "file://" + System.IO.Path.Combine(Application.streamingAssetsPath, "/" + file_name);
    #endif
        }
    
        public static string getLocalHostURLFile(string folder_name, string file_name)
        {
    #if !UNITY_EDITOR && UNITY_WEBGL
                return "http://localhost:8080/StreamingAssets/" + folder_name + "/" + file_name;
    #else
            return "file://" + System.IO.Path.Combine(Application.streamingAssetsPath, "/" + folder_name + "/" + file_name);
    #endif
        }
    
        public static string GetCallerName([CallerMemberName] string caller = null)
        {
            return caller;
        }
    
        /**
             * <summary> Restituisce l'elenco dei file presenti nella cartella passata in input, che risiede dentro StreamingAssets.
            </summary>
            <remarks>La cartella è quella del server, non quella nota a tempo di build (quindi se vengono aggiunti file dopo la creazione della build questi verranno inseriti nella lista di ritorno</remarks>
             */
        public static IEnumerator GetServerFilesInFolder(string folder, Action<List<string>> callback)
        {
            yield return GetFilesInServerDirectory(folder);
            callback.Invoke(JsonConvert.DeserializeObject<List<string>>(jsonFile));
        }
    
    
        static IEnumerator GetFilesInServerDirectory(string folder)
        {
            #if !UNITY_EDITOR && UNITY_WEBGL
                var url = "http://localhost:8080/" + _serverGetFolder;
            #else
                var url = getStreamingAssetsURL()+folder+"/";
            #endif
            
            using (UnityWebRequest uwr = UnityWebRequest.Get(url + folder))
            {
                yield return uwr.SendWebRequest();
                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(
                        "C'è stato un problema nella richiesta HTTP. Verifica che la cartella richiesta esista sul server.");
                    jsonFile = "";
                }
                else
                {
                    jsonFile = uwr.downloadHandler.text;
                }
            }
    
            yield return null;
        }
    
        public static void PrintStringList(List<string> list)
        {
            foreach (var element in list)
            {
                Debug.Log(element);
            }
        }
        
        /// <summary>
        /// Carica una texture da URL. Restituisce, tramite una callback, una tupla (Texture2D, string) rappresentante la texture e il path dell'immagine
        /// </summary>
        /// <param name="fileName">Nome dell'immagine con cui creare il materiale</param>
        /// <param name="callback"> Rappresenta il parametro di ritorno. Essendo una coroutine, non è possibile ritornare alcun valore.</param>
        /// <returns></returns>
        public static IEnumerator LoadTextureFromURL(string fileName, Action<Tuple<Texture2D, string>> callback)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(TaxonomyUtils.getFileImageByName(fileName));
    
            yield return www.SendWebRequest();
    
            if (www.result != UnityWebRequest.Result.Success)
            {
                TaxonomyUtils.AgileErrorStatus(TaxonomyUtils.CONNECTION_ERROR, "", "");
                Debug.LogError(
                    "Si è verificato un errore nel caricamento della texture. Verifica che il file esista e riprova.");
            }
    
            else
            {
                Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                var tupleResult = new Tuple<Texture2D, string>(texture, fileName) ;
                callback(tupleResult);
            }
        }   
    }
}
