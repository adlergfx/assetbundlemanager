# Asset Bundle Manager

Der Asset Bundle Manager (ABM) erlaubt das Nachladen von AssetBundles mit Skripten, um Funktionen nachzuladen oder 
Ingame zu aktualisieren. Die Idee ist, dass ein AssetBundle erstellt wird mit Skripten. Die Skripte werden in eine
Assembly kompiliert. Ein Editor Skript erstellt das Bundle, zippt es mit der entsprechenden Assembly und generiert
eine einfache JSON-Begleitdatei mit der Versionsnummer. 

Das Zip mit der gleichnamigen Json wird auf einen Server geladen. 

Zur Laufzeit wird die JSON geprüft, ob dort eine aktuellere Version vorliegt. Wenn ja, dann wird die zip in das
streamingAssets Verzeichnis geladen und entpackt. Anschließend wird die Assembly.dll geladen und erst dann das
Bundle, so dass Abhängigkeiten gegeben sind. 

## Erstellen eines Bundle

1. Ordner erstellen für das Bundle
2. Alle bundlespezifischen Skripte in einen Unterordner (z.B. Scripts wie üblich)
3. AssemblyDefinition Datei im Skripte Ordner anlegen. Der Name sollte dem Bundlenamen entsprechen
    - Die AssemblyDefinition legt fest dass alle Skripte in eine dll mit dem Namen des Bundle kompiliert werden. 
    - Bundles können hierarchisch organisiert sein. Wenn der Bundlename "mybundle" ist, muss die AssemblyDefinition
      genauso heissen. 
    - Wenn der Bundlename "path/path/mybundle" heisst, wird die AssemblyDefinition auch mit "mybundle" bezeichnet. 
5. Dem Ordner in dem sich alle Inhalte befinden dem AssetBundle zuordnen. 
6. Menu: Editor/Build Asset Bundles
    - Das Skript erstellt je Bundle eine Zip mit begleitender JSON-Dateien mit Meta-Daten

## Laden eines Bundle

1. GameObject mit InteractiveLoader erstellen
   Der Loader legt fest von welchem Server die zip-Dateien geladen werden und welche Zip-Dateien geladen werden.
   - Nach Laden des Skriptes wird eine Callback aufgerufen, der der geladene Packagemname übergeben wird.
   - Das auf die Callback registrierte Skript muss Kenntniss haben, welches Skript bezogen wird
2. Meist will man ein zentrales Prefab aus dem Bundle laden. In diesem Fall kann das Skript "Prefab Handler" genutzt werden. 
   - Es wird der Package Name angegeben auf den gewartet wird (Name des zip)
   - Das Bundle innerhalb des Package, das zu laden ist (normalerweise Name des zip ODER bundle mit Pfad)
   - Der Name des zu Instantiierenden Prefab, das dann als Kindelement des Prefab Handlers eingefügt wird
