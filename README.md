# MAIN MENU:
La intrarea în aplicație apare meniul principal al jocului unde se pot selecta 3 butoane:

•	NEW GAME – la apăsarea acestui buton se deschide altă scena în care se află jocul.

•	OPTIONS – la apăsarea acestui buton se deschide un al doilea meniu în care se poate controla volumul muzicii de fundal și un buton BACK care revine la meniul principal.

•	EXIT – la apăsarea acestui buton se închide aplicația.
 ![image](https://user-images.githubusercontent.com/77692523/223123680-c9c18ab2-9b31-4620-9394-86bca6a76d50.png)
 
# OPTIONS:
Setarea volumului sunetului la intrarea in joc și modificarea lui la tragerea sliderului:
```csharp
public class VolumeValueChange : MonoBehaviour
{
    [SerializeField] Slider slider;
    void Awake()
    {
        if(PlayerPrefs.HasKey("Volume"))
        {
            SetVolume(PlayerPrefs.GetFloat("Volume"));
            slider.value = PlayerPrefs.GetFloat("Volume");
        }
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);
    }
}
```

Clasa în care muzica este transmisă la scena jocului:
```csharp
public class HandlerInputAudio : MonoBehaviour
{
    public static HandlerInputAudio instanta;
    void Awake()
    {
        if (instanta == null)
            instanta = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(transform.gameObject);
    }
}
```

# Chess (Scena Jocului):
La intrare în scenă se apelează funcția StartJocNou din clasa ChessGameController care creează piesele și generează mutările posibile ale jucătorului activ (Alb):
```csharp
private void Start()
{
    StartJocNou();
}

private void StartJocNou()
{
    Camera.main.transform.position = new Vector3(0f, 14f, -11.5f);
    Camera.main.transform.rotation = Quaternion.Euler(55, 0, 0);
    SetareStatutJoc(StatutJoc.Inceput);
    UIManager.HideUI();
    PromovarePion.HideUI();
    tabla.SetareDependente(this);
    CrearePieseDinLayout(layoutTablaInceput);
    playerActiv = playerAlb;
    GenerareMutariPosibileAleJucatorului(playerActiv);
    SetareStatutJoc(StatutJoc.Jucare);
}
```

Clasa InputColliderReciever detectează apăsarea mouse-ului pe ecran și apelează funcția ProcesareInput din fiecare clasă derivată interfeței IInputHandler:
```csharp
public class InputColliderReciever : InputReciever
{
    private Vector3 pozitieClick;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit))
            {
                pozitieClick = hit.point;
                OnInputRecieved();
            }
        }
    }
    public override void OnInputRecieved()
    {
        foreach(var handler in inputHandleri)
        {
            handler.ProcesareInput(pozitieClick, null, null);
        }
    }
}
```

Clasa HandlerInputTabla apelează funcția LaPătratSelectat, din clasa Tabla, la apăsarea mouse-ului pe ecran:
```csharp
public class HandlerInputTabla : MonoBehaviour, IInputHandler
{
    private Tabla tabla;

    private void Awake()
    {
        tabla = GetComponent<Tabla>();
    }

    public void ProcesareInput(Vector3 pozitieInput, GameObject obiectSelectat, Action callback)
    {
        tabla.LaPatratSelectat(pozitieInput);
    }
}
```

În funcția LaPătratSelectat din clasa Tabla, dacă există deja o piesă selectată se verifică dacă aceasta este piesa de pe pozițiaInput (poziția mouse-ului la apăsare). În acest caz, se deselectează piesa iar în caz contrar, dacă există o piesă, de aceeași culoare cu a piesei selectate, pe coordonatele pozitieiInput se selectează acea piesă, altfel se mută piesa selectată pe acea poziție. Dacă nu există o piesă selectată și dacă există pe pozițiaInput o piesă de aceeași culoare cu a jucătorului activ, se selectează acea piesă.
```csharp
public void LaPatratSelectat(Vector3 pozitieInput)
{
    if (!chessController.EsteJoculInDesfasurare())
        return;
    Vector2Int coord = CalculareCooronateDinPozitie(pozitieInput);
    Piesa piesa = PiesaPatrat(coord);

    if (piesaSelectata)
    {
        if (piesa != null && piesaSelectata == piesa)
        {
            DeselectarePiesa();
        }
        else if (piesa != null && piesaSelectata != piesa && chessController.EsteRandulEchipeiActive(piesa.echipa))
            SelectarePiesa(piesa);
        else if (piesaSelectata.PoateMutaLa(coord))
        {
            LaPiesaSelectataMutata(coord, piesaSelectata);
        }
    }
    else
    {
        if (piesa != null && chessController.EsteRandulEchipeiActive(piesa.echipa))
            SelectarePiesa(piesa);
    }
}
```

Funcția SelectarePiesă elimină mutările posibile ale piesei care pot permite atacarea regelui de către oponent și, prin apelarea funcției AratăPatrateSelecție arată, pe fiecare pătrat din mutările posibile, un selector.
 ```csharp
private void SelectarePiesa(Piesa piesa)
{
    chessController.EliminaMutariCarePermitAtacareaPiesei<Rege>(piesa);
    piesaSelectata = piesa;
    List<Vector2Int> selectie = piesaSelectata.mutariPosibile;
    ArataPatrateSelectie(selectie);
}
```
 
Clasele Pion, Cal, Nebun, Turn, Regina, Rege returnează mutările posibile pentru fiecare din tipurile de piese. De exemplu Clasa Cal returnează toate mutările în L posibile a pieselor de tip cal (cele în care coordonatele următoare se afle pe tablă): 
```csharp
public class Cal : Piesa
{
    Vector2Int[] offseturi = new Vector2Int[]
    {
        new Vector2Int(2, 1),
        new Vector2Int(2, -1),
        new Vector2Int(1, 2),
        new Vector2Int(1, -2),
        new Vector2Int(-2, 1),
        new Vector2Int(-2, -1),
        new Vector2Int(-1, 2),
        new Vector2Int(-1, -2),
    };
    public override List<Vector2Int> SelectarePatratePosibile()
    {
        mutariPosibile.Clear();
        for(int i = 0; i < offseturi.Length; i++)
        {
            Vector2Int coordUrmatoare = patratOcupat + offseturi[i];
            Piesa piesa = tabla.PiesaPatrat(coordUrmatoare);
            if (!tabla.VerificareDacaCoordonateleSuntPeTabla(coordUrmatoare))
                continue;
            if(piesa == null || !piesa.AreAceeasiCuloare(this))
                IncercareAdaugareMutare(coordUrmatoare);
        }
        return mutariPosibile;
    }
}
```


# Mutări speciale:
## 1.	Rocada

Regele poate efectua doua mutări speciale, numite rocada mică și rocada mare. Rocada este o mutare specială, făcută de rege și de unul dintre turnuri. Ea constă în mutarea regelui două poziții spre turn și mutarea turnului pe poziția peste care a sărit regele. Când turnul este mutat doar două poziții se efectuează rocada mică, iar când este mutat trei poziții se efectuează rocada mare. Rocadele sunt posibile numai în cazul în care:
-	 nici regele, nici turnul nu au fost mutați de la începutul jocului
-	regele nu a trecut prin șah până la ajungerea la poziția finală din rocadă


În funcția AplicăMutăriRocadă din clasa Rege se verifică prima condiție de sus. În cazul în care aceasta se îndeplinește, se adaugă mutarea în mutăriPosibile.
```csharp
private void AplicaMutariRocada()
{
    rocadaLaStanga = new Vector2Int(-1, -1);
    rocadaLaDreapta = new Vector2Int(-1, -1);
    if (!aFostMutat)
    {
        turnStanga = PrimestePiesaDinDirectia<Turn>(echipa, Vector2Int.left);
        if (turnStanga && !turnStanga.aFostMutat)
        {
            rocadaLaStanga = patratOcupat + Vector2Int.left * 2;
            mutariPosibile.Add(rocadaLaStanga);
        }

        turnDreapta = PrimestePiesaDinDirectia<Turn>(echipa, Vector2Int.right);
        if (turnDreapta && !turnDreapta.aFostMutat)
        {
            rocadaLaDreapta = patratOcupat + Vector2Int.right * 2;
            mutariPosibile.Add(rocadaLaDreapta);
        }
    }
}
```
 
În funcția EliminăMutăriCarePermitAtacareaPiesei din clasa ChessPlayer este verificată condiția a doua și în cazul neîndeplinirii ei se elimină mutarea din mutăriPosibile:
```csharp
if (piesaSelectata is Rege)
{
    foreach (var patrat in piesaSelectata.mutariPosibile)
        if (piesaSelectata.patratOcupat + Vector2Int.left * 2 == patrat)
        {
            if (oponent.VerificaDacaOponentAtacaPiesa<T>())
            { 
                coordPentruEliminare.Add(piesaSelectata.patratOcupat + Vector2Int.left * 2);
            }
            Piesa piesaPePatrat = tabla.PiesaPatrat(piesaSelectata.patratOcupat + Vector2Int.left);
            tabla.UpdateTablaLaMutarePiesa(piesaSelectata.patratOcupat + Vector2Int.left, piesaSelectata.patratOcupat, piesaSelectata, null);
            oponent.GenerareMutariPosibile();
            if (oponent.VerificaDacaOponentAtacaPiesa<T>())
                coordPentruEliminare.Add(piesaSelectata.patratOcupat + Vector2Int.left * 2);
            tabla.UpdateTablaLaMutarePiesa(piesaSelectata.patratOcupat, piesaSelectata.patratOcupat + Vector2Int.left, piesaSelectata, piesaPePatrat);
        }

    foreach (var patrat in piesaSelectata.mutariPosibile)
        if (piesaSelectata.patratOcupat + Vector2Int.right * 2 == patrat)
        {
            if (oponent.VerificaDacaOponentAtacaPiesa<T>())
            {
                coordPentruEliminare.Add(piesaSelectata.patratOcupat + Vector2Int.right * 2);
            }
            Piesa piesaPePatrat = tabla.PiesaPatrat(piesaSelectata.patratOcupat + Vector2Int.right);
            tabla.UpdateTablaLaMutarePiesa(piesaSelectata.patratOcupat + Vector2Int.right, piesaSelectata.patratOcupat, piesaSelectata, null);
            oponent.GenerareMutariPosibile();
            if (oponent.VerificaDacaOponentAtacaPiesa<T>())
                coordPentruEliminare.Add(piesaSelectata.patratOcupat + Vector2Int.right * 2);
            tabla.UpdateTablaLaMutarePiesa(piesaSelectata.patratOcupat, piesaSelectata.patratOcupat + Vector2Int.right, piesaSelectata, piesaPePatrat);
        }
}

foreach (var coord in coordPentruEliminare)
{
    piesaSelectata.mutariPosibile.Remove(coord);
}
```


## 2.	En Passant

Regula En passant se aplică atunci când un jucător mută unul dintre pionii săi două poziții înainte și trece pe lângă un pion advers de către care ar fi putut fi capturat dacă ar fi fost mutat doar o poziție înainte. Regula spune că pionul care a efectuat mutarea poate fi capturat de cel advers doar la mutarea următoare, ca și când s-ar fi deplasat doar o poziție înainte. Dacă nu este capturat la prima mutare a adversarului, acesta pierde dreptul de a-l mai captura.
Mutarea este neobișnuită pentru că este singura din jocul de șah în care piesa ce capturează nu rămâne pe câmpul pe care s-a aflat piesa capturată.

În funcția SelectarePatratePosibile a clasei Pion sunt verificare condițiile pentru mutarea EnPassant:
 ```csharp
int nrEnPassant = 0;

for (int i = 0; i < directiiCapturare.Length; i++)
{
    Vector2Int coordUrmatoare = patratOcupat + directiiCapturare[i];
    Vector2Int coordUrmatoareEnPassant = coordUrmatoare - directie;
    Piesa piesa = tabla.PiesaPatrat(coordUrmatoareEnPassant);
    if (!tabla.VerificareDacaCoordonateleSuntPeTabla(coordUrmatoare))
    {
        nrEnPassant = 1;
        break;
    }
    if (piesa != null && !piesa.AreAceeasiCuloare(this) && coordUrmatoare.y == 5 && piesa.echipa == CuloareEchipa.Negru && piesa.aFostMutatPentruPrimaData && piesa is Pion)
    { 
        IncercareAdaugareMutare(coordUrmatoare);
        piesa.aFostMutatPentruPrimaData = false;
    }
    if (piesa != null && !piesa.AreAceeasiCuloare(this) && coordUrmatoare.y == 2 && piesa.echipa == CuloareEchipa.Alb && piesa.aFostMutatPentruPrimaData && piesa is Pion)
    {
        IncercareAdaugareMutare(coordUrmatoare);
        piesa.aFostMutatPentruPrimaData = false;
    }
}

if (nrEnPassant == 1)
{
    Vector2Int coordUrmatoare = patratOcupat + directiiCapturare[1];
    Piesa piesa = tabla.PiesaPatrat(coordUrmatoare - directie);
    if (piesa != null && !piesa.AreAceeasiCuloare(this) && coordUrmatoare.y == 5 && piesa.echipa == CuloareEchipa.Negru && piesa.aFostMutatPentruPrimaData && piesa is Pion)
    { 
        IncercareAdaugareMutare(coordUrmatoare);
        piesa.aFostMutatPentruPrimaData = false;
    }
    if (piesa != null && !piesa.AreAceeasiCuloare(this) && coordUrmatoare.y == 2 && piesa.echipa == CuloareEchipa.Alb && piesa.aFostMutatPentruPrimaData && piesa is Pion)
    { 
        IncercareAdaugareMutare(coordUrmatoare);
        piesa.aFostMutatPentruPrimaData = false;
    }
}
```
 

## 3.	Promovarea pionului

Promovarea pionului înseamnă transformarea acestuia atunci când este mutat pe ultima linie, la alegerea jucătorului, în regina, turn, cal sau nebun de aceeași culoare. Noua piesă înlocuiește pionul pe aceeași poziție.
Am creat un meniu cu 4 butoane care se deschide când ajunge un pion pe ultima linie. Butoanele reprezintă piesele în care se poate promova pionul (Cal, Nebun, Turn, Regină). La apăsarea unui buton, pionul promovează în piesa corespunzătoare butonului. 
 ![image](https://user-images.githubusercontent.com/77692523/223124351-1b98d539-baeb-4876-8949-e9c1c3426acd.png)
 
Funcția VerificarePromovare din clasa Pion verifică dacă există un pion pe prima sau ultima linie. În acest caz funcția deschide meniul:
```csharp
private void VerificarePromovare()
{
    int coordYPromovare = echipa == CuloareEchipa.Alb ? Tabla.Marime_Tabla - 1 : 0;
    if (patratOcupat.y == coordYPromovare)
    {
        tabla.PromovareShow();
    }
}
```
 
Când regele se află în șah, acesta trebuie să iasă din șah sau o altă piesă să îl apere de piesa atacatoare. În acest caz celelalte piese, care nu pot apăra regele de șah, nu vor avea mutări posibile. Această condiție se verifică după fiecare mutare, în funcția EliminăMutăriCarePermitAtacareaPiesei din clasa ChessPlayer și dacă regele este în șah, se elimină, cum am zis mai sus, toate mutările pieselor care nu pot apăra regele:
```csharp
public void EliminaMutariCarePermitAtacareaPiesei<T>(ChessPlayer oponent, Piesa piesaSelectata) where T : Piesa
{
    List<Vector2Int> coordPentruEliminare = new List<Vector2Int>();
    foreach(var coord in piesaSelectata.mutariPosibile)
    {
        Piesa piesaPePatrat = tabla.PiesaPatrat(coord);
        tabla.UpdateTablaLaMutarePiesa(coord, piesaSelectata.patratOcupat, piesaSelectata, null);
        oponent.GenerareMutariPosibile();
        if (oponent.VerificaDacaOponentAtacaPiesa<T>())
            coordPentruEliminare.Add(coord);
        tabla.UpdateTablaLaMutarePiesa(piesaSelectata.patratOcupat, coord, piesaSelectata, piesaPePatrat);
    }
```
 
După fiecare mutare, camera se rotește în jurul tablei schimbându-și perspectiva de la poziția jucătorului alb la cel negru și invers. Funcția care acționează animațiile camerei este EndTurn din clasa ChessGameController. În funcție se mai verifică dacă jocul s-a finalizat.
```csharp
public void EndTurn()
{
    SchimbarePiesaPromovare();
    GenerareMutariPosibileAleJucatorului(playerActiv);
    GenerareMutariPosibileAleJucatorului(PlayerOponent(playerActiv));
    if (VerificareSfarsitJoc())
        SfarsitJoc();
    else
    {
        SchimbaEchipaActiva();
        if (playerActiv == playerNegru)
            Camera.main.GetComponent<Animation>().Play("RotireCameraAlb");
        else
            Camera.main.GetComponent<Animation>().Play("RotireCameraNegru");
    }
}
```
 
 # Condiții de finalizare joc
Jocul se încheie când se întâlnește una din următoarele condiții
-	Un rege se află în șah și nu se mai poate apăra, adică jucătorul nu are mutări posibile (Șah mat). Câștigă jucătorul adversar regelui în șah mat.
-	Remiză. Aceasta se întâlnește când un jucător nu are mutări posibile iar regele nu se află în șah (pat) respectiv în cazul în care niciun jucător nu mai are material cu care este posibil un șah mat.
Aceste condiții sunt verificate în funcția VerificareSfârșitJoc din clasa ChessGameController apelată în EndTurn. Funcția VerificareSfârșitJoc este de tip bool și returnează true în cazul în care găsește una din condițiile de mai sus. În acest caz, în EndTurn, se apelează funția SfârșitJoc, altfel funcția VerificareSfârșitJoc returnează false. Prima secvență a funcției verifică dacă un rege se află în șah mat:

```csharp
pieseAtacatoare = playerActiv.PieseCareAtacaPieseOponenteDeTipul<Rege>();
if (pieseAtacatoare.Length > 0)
{
    ChessPlayer playerOponent = PlayerOponent(playerActiv);
    Piesa regeAtacat = playerOponent.PrimestePieseDeTipul<Rege>().FirstOrDefault();
    playerOponent.EliminaMutariCarePermitAtacareaPiesei<Rege>(playerActiv, regeAtacat);

    int mutariPosibileAleRegelui = regeAtacat.mutariPosibile.Count;
    if (mutariPosibileAleRegelui == 0)
    {
        bool poateAparaRegele = playerOponent.PoateAparaPiesaDeAtac<Rege>(playerActiv);
        if (!poateAparaRegele)
            return true;
    }
}
```
 
A doua secvență verifică existența unei remize în care un jucător nu are mutări posibile iar regele nu se află în șah (pat):
```csharp
else
{
    ChessPlayer playerOponent = PlayerOponent(playerActiv);
    int nr = 0;
    foreach (var piesa in playerOponent.pieseActive)
    {
        nr = nr + piesa.mutariPosibile.Count;
    }
    List<Vector2Int> coordPentruEliminare = new List<Vector2Int>();
    foreach (var piesa in playerOponent.pieseActive)
    {
        foreach (var coord in piesa.mutariPosibile)
        {
            Piesa piesaPePatrat = tabla.PiesaPatrat(coord);
            tabla.UpdateTablaLaMutarePiesa(coord, piesa.patratOcupat, piesa, null);
            playerActiv.GenerareMutariPosibile();
            if (playerActiv.VerificaDacaOponentAtacaPiesa<Rege>())
                coordPentruEliminare.Add(coord);
            tabla.UpdateTablaLaMutarePiesa(piesa.patratOcupat, coord, piesa, piesaPePatrat);
        }
    }
    if (nr == coordPentruEliminare.Count)
        return true;
```
 
A treia secvență a funcției verifică dacă există remiză prin material insuficient:
```csharp
    if (playerActiv.pieseActive.Count == 2 && playerOponent.pieseActive.Count == 1)
    {
        if (playerActiv.pieseActive[0] is Nebun || playerActiv.pieseActive[1] is Nebun)
            return true;
        else if (playerActiv.pieseActive[0] is Cal || playerActiv.pieseActive[1] is Cal)
            return true;
    }
    else if (playerActiv.pieseActive.Count == 1 && playerOponent.pieseActive.Count == 2)
    {
        if (playerOponent.pieseActive[0] is Nebun || playerOponent.pieseActive[1] is Nebun)
            return true;
        else if (playerOponent.pieseActive[0] is Cal || playerOponent.pieseActive[1] is Cal)
            return true;
    }
    else if (playerActiv.pieseActive.Count == 1 && playerOponent.pieseActive.Count == 1)
        return true;
}
return false;
```

Funcția SfârșitJoc din clasa ChessGameController apelează funcția LaJocÎncheiat având ca parametru jucătorul câștigător în cazul unui câștig, sau remiză în cazul unei remize: 
```csharp
private void SfarsitJoc()
{
    SetareStatutJoc(StatutJoc.Sfarsit);
    ChessPlayer playerOponent = PlayerOponent(playerActiv);
    
    int nr = 0;
    foreach (var piesa in playerOponent.pieseActive)
    {
        nr = nr + piesa.mutariPosibile.Count();
    }
    List<Vector2Int> coordPentruEliminare = new List<Vector2Int>();
    foreach (var piesa in playerOponent.pieseActive)
    {
        foreach (var coord in piesa.mutariPosibile)
        {
            Piesa piesaPePatrat = tabla.PiesaPatrat(coord);
            tabla.UpdateTablaLaMutarePiesa(coord, piesa.patratOcupat, piesa, null);
            playerActiv.GenerareMutariPosibile();
            if (playerActiv.VerificaDacaOponentAtacaPiesa<Rege>())
                coordPentruEliminare.Add(coord);
            tabla.UpdateTablaLaMutarePiesa(piesa.patratOcupat, coord, piesa, piesaPePatrat);
        }
    }
    Piesa regeAtacat = playerOponent.PrimestePieseDeTipul<Rege>().FirstOrDefault();
    playerOponent.EliminaMutariCarePermitAtacareaPiesei<Rege>(playerActiv, regeAtacat);

    int nrMaterialInsuficient = 0;
    if (playerActiv.pieseActive.Count == 2 && playerOponent.pieseActive.Count == 1)
    {
        if (playerActiv.pieseActive[0] is Nebun || playerActiv.pieseActive[1] is Nebun)
            nrMaterialInsuficient = 1;
        else if (playerActiv.pieseActive[0] is Cal || playerActiv.pieseActive[1] is Cal)
            nrMaterialInsuficient = 1;
    }
    else if (playerActiv.pieseActive.Count == 1 && playerOponent.pieseActive.Count == 2)
    {
        if (playerOponent.pieseActive[0] is Nebun || playerOponent.pieseActive[1] is Nebun)
            nrMaterialInsuficient = 1;
        else if (playerOponent.pieseActive[0] is Cal || playerOponent.pieseActive[1] is Cal)
            nrMaterialInsuficient = 1;
    }
    else if (playerActiv.pieseActive.Count == 1 && playerOponent.pieseActive.Count == 1)
        nrMaterialInsuficient = 1;
    if (nr == coordPentruEliminare.Count && pieseAtacatoare.Length == 0)
    {
        UIManager.LaJocIncheiat("Remiză");
    }
    else if (nrMaterialInsuficient == 1)
    { 
        UIManager.LaJocIncheiat("Remiză");
    }
    else
        UIManager.LaJocIncheiat(playerActiv.echipa.ToString());
}
```
 
Funcția LaJocÎncheiat din clasa ChessUIManager setează rezultatul jocului și deschide un meniu care arată rezultatul jocului:
```csharp
internal void LaJocIncheiat(string castigator)
{
    UIParent.SetActive(true);
    if(castigator == "Remiză")
        rezultat.text = string.Format("Draw");
    else
    {
        if(castigator == "Alb")
            rezultat.text = string.Format("White won");
        else
            rezultat.text = string.Format("Black won");
    }
}
```
 
Exemplu – meniul la câștigarea jucătorului negru:
 ![image](https://user-images.githubusercontent.com/77692523/223124666-d5bcd47c-7ffb-4b5a-81fa-485173079f28.png)
 
Meniul mai conține două butoane. Unul de RESTART care începe o nouă partidă și unul de BACK care revine la meniul principal al aplicației.




Butonul RESTART are o acțiune onClick care apelează funcția RestartJoc din clasa ChessGameController. Aceasta distruge piesele din jocul anterior și creează unele noi după layout-ul inițial:
```csharp
public void RestartJoc()
{
    if(playerActiv == playerNegru && statut == StatutJoc.Jucare)
        Camera.main.GetComponent<Animation>().Play("RotireCameraNegru");
    DistrugerePiese();
    tabla.LaJocRestartat();
    playerAlb.LaJocRestartat();
    playerNegru.LaJocRestartat();
    StartJocNou();
}
```
 
Butonul BACK are o acțiune onClick care apelează funcția ExitUI din clasa UIMenu care revine la meniul principal la aplicației:
```csharp
public void ExitUI()
{
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
}
```
 
În timpul jocului, în partea dreapta jos a ecranului se află două butoane, NEW GAME și MAIN MENU. Butonul NEW GAME are o acțiune onClick care apelează funcția RestartJoc din clasa ChessGameController. În cazul în care jucătorul activ este cel negru la apăsarea butonului, camera își schimbă perspectiva la perspectiva jucătorului alb. Butonul MAIN MENU are o acțiune onClick care apelează funcția ExitUI din clasa UIMenu care revine la meniul principal la aplicației.
