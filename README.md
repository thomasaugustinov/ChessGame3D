# MAIN MENU:
La intrarea în aplicație apare meniul principal al jocului unde se pot selecta 3 butoane:

•	NEW GAME – la apăsarea acestui buton se deschide altă scena în care se află jocul.

•	OPTIONS – la apăsarea acestui buton se deschide un al doilea meniu în care se poate controla volumul muzicii de fundal și un buton BACK care revine la meniul principal.

•	EXIT – la apăsarea acestui buton se închide aplicația.
 ![image](https://user-images.githubusercontent.com/77692523/223123680-c9c18ab2-9b31-4620-9394-86bca6a76d50.png)
 
# OPTIONS:
Setarea volumului sunetului la intrarea in joc și modificarea lui la tragerea sliderului:
![image](https://user-images.githubusercontent.com/77692523/223123755-a9900a9e-14e0-4da3-aaf3-844cec7d4bef.png)

Clasa în care muzica este transmisă la scena jocului:
![image](https://user-images.githubusercontent.com/77692523/223123823-e5105297-a68a-46ef-8fab-e76ded833344.png)

# Chess (Scena Jocului):
La intrare în scenă se apelează funcția StartJocNou din clasa ChessGameController care creează piesele și generează mutările posibile ale jucătorului activ (Alb):
![image](https://user-images.githubusercontent.com/77692523/223123907-1d226106-ddab-480f-afc5-cb4f67cfe31c.png)

Clasa InputColliderReciever detectează apăsarea mouse-ului pe ecran și apelează funcția ProcesareInput din fiecare clasă derivată interfeței IInputHandler:

![image](https://user-images.githubusercontent.com/77692523/223123935-6189f0b7-1035-474c-8e18-3b19a292d9e0.png)

Clasa HandlerInputTabla apelează funcția LaPătratSelectat, din clasa Tabla, la apăsarea mouse-ului pe ecran:

![image](https://user-images.githubusercontent.com/77692523/223123982-c8b217db-bdc2-474f-9d48-fa6777c2ddc8.png)

În funcția LaPătratSelectat din clasa Tabla, dacă există deja o piesă selectată se verifică dacă aceasta este piesa de pe pozițiaInput (poziția mouse-ului la apăsare). În acest caz, se deselectează piesa iar în caz contrar, dacă există o piesă, de aceeași culoare cu a piesei selectate, pe coordonatele pozitieiInput se selectează acea piesă, altfel se mută piesa selectată pe acea poziție. Dacă nu există o piesă selectată și dacă există pe pozițiaInput o piesă de aceeași culoare cu a jucătorului activ, se selectează acea piesă.
![image](https://user-images.githubusercontent.com/77692523/223124018-b2adb4fe-1079-4972-98c9-2ef44fa44f67.png)

Funcția SelectarePiesă elimină mutările posibile ale piesei care pot permite atacarea regelui de către oponent și, prin apelarea funcției AratăPatrateSelecție arată, pe fiecare pătrat din mutările posibile, un selector.
 ![image](https://user-images.githubusercontent.com/77692523/223124077-35c18621-3c61-4c9a-91b4-4a920ba8ee06.png)
 
Clasele Pion, Cal, Nebun, Turn, Regina, Rege returnează mutările posibile pentru fiecare din tipurile de piese. De exemplu Clasa Cal returnează toate mutările în L posibile a pieselor de tip cal (cele în care coordonatele următoare se afle pe tablă): 
![image](https://user-images.githubusercontent.com/77692523/223124108-c37e7df1-39f4-4028-8c28-08c0a2865478.png)


# Mutări speciale:
## 1.	Rocada

Regele poate efectua doua mutări speciale, numite rocada mică și rocada mare. Rocada este o mutare specială, făcută de rege și de unul dintre turnuri. Ea constă în mutarea regelui două poziții spre turn și mutarea turnului pe poziția peste care a sărit regele. Când turnul este mutat doar două poziții se efectuează rocada mică, iar când este mutat trei poziții se efectuează rocada mare. Rocadele sunt posibile numai în cazul în care:
-	 nici regele, nici turnul nu au fost mutați de la începutul jocului
-	regele nu a trecut prin șah până la ajungerea la poziția finală din rocadă


În funcția AplicăMutăriRocadă din clasa Rege se verifică prima condiție de sus. În cazul în care aceasta se îndeplinește, se adaugă mutarea în mutăriPosibile.
 ![image](https://user-images.githubusercontent.com/77692523/223124173-c3c9ed68-de03-4cbd-a85b-7c2da7044b42.png)
 
În funcția EliminăMutăriCarePermitAtacareaPiesei din clasa ChessPlayer este verificată condiția a doua și în cazul neîndeplinirii ei se elimină mutarea din mutăriPosibile:
![image](https://user-images.githubusercontent.com/77692523/223124223-8c24dfb2-f559-4f08-9e99-1f5c87adc8f8.png)


## 2.	En Passant

Regula En passant se aplică atunci când un jucător mută unul dintre pionii săi două poziții înainte și trece pe lângă un pion advers de către care ar fi putut fi capturat dacă ar fi fost mutat doar o poziție înainte. Regula spune că pionul care a efectuat mutarea poate fi capturat de cel advers doar la mutarea următoare, ca și când s-ar fi deplasat doar o poziție înainte. Dacă nu este capturat la prima mutare a adversarului, acesta pierde dreptul de a-l mai captura.
Mutarea este neobișnuită pentru că este singura din jocul de șah în care piesa ce capturează nu rămâne pe câmpul pe care s-a aflat piesa capturată.

În funcția SelectarePatratePosibile a clasei Pion sunt verificare condițiile pentru mutarea EnPassant:
 ![image](https://user-images.githubusercontent.com/77692523/223124306-be363a8a-16de-4ad4-9f6e-bf712e9086d8.png)
 

## 3.	Promovarea pionului

Promovarea pionului înseamnă transformarea acestuia atunci când este mutat pe ultima linie, la alegerea jucătorului, în regina, turn, cal sau nebun de aceeași culoare. Noua piesă înlocuiește pionul pe aceeași poziție.
Am creat un meniu cu 4 butoane care se deschide când ajunge un pion pe ultima linie. Butoanele reprezintă piesele în care se poate promova pionul (Cal, Nebun, Turn, Regină). La apăsarea unui buton, pionul promovează în piesa corespunzătoare butonului. 
 ![image](https://user-images.githubusercontent.com/77692523/223124351-1b98d539-baeb-4876-8949-e9c1c3426acd.png)
 
Funcția VerificarePromovare din clasa Pion verifică dacă există un pion pe prima sau ultima linie. În acest caz funcția deschide meniul:
 ![image](https://user-images.githubusercontent.com/77692523/223124375-81a354f4-3ec3-42ad-a135-1c50de7a808a.png)
 
Când regele se află în șah, acesta trebuie să iasă din șah sau o altă piesă să îl apere de piesa atacatoare. În acest caz celelalte piese, care nu pot apăra regele de șah, nu vor avea mutări posibile. Această condiție se verifică după fiecare mutare, în funcția EliminăMutăriCarePermitAtacareaPiesei din clasa ChessPlayer și dacă regele este în șah, se elimină, cum am zis mai sus, toate mutările pieselor care nu pot apăra regele:
 ![image](https://user-images.githubusercontent.com/77692523/223124414-2c2aaaaf-e532-4e73-8cb0-c1d2d12cade0.png)
 
După fiecare mutare, camera se rotește în jurul tablei schimbându-și perspectiva de la poziția jucătorului alb la cel negru și invers. Funcția care acționează animațiile camerei este EndTurn din clasa ChessGameController. În funcție se mai verifică dacă jocul s-a finalizat.
 ![image](https://user-images.githubusercontent.com/77692523/223124440-15fbd7ea-5e2c-4095-8077-5327874fd133.png)
 
Jocul se încheie când se întâlnește una din următoarele condiții
-	Un rege se află în șah și nu se mai poate apăra, adică jucătorul nu are mutări posibile (Șah mat). Câștigă jucătorul adversar regelui în șah mat.
-	Remiză. Aceasta se întâlnește când un jucător nu are mutări posibile iar regele nu se află în șah (pat) respectiv în cazul în care niciun jucător nu mai are material cu care este posibil un șah mat.
Aceste condiții sunt verificate în funcția VerificareSfârșitJoc din clasa ChessGameController apelată în EndTurn. Funcția VerificareSfârșitJoc este de tip bool și returnează true în cazul în care găsește una din condițiile de mai sus. În acest caz, în EndTurn, se apelează funția SfârșitJoc, altfel funcția VerificareSfârșitJoc returnează false. Prima secvență a funcției verifică dacă un rege se află în șah mat:

 ![image](https://user-images.githubusercontent.com/77692523/223124487-c2c5037a-5716-4e57-9712-4d4e780d629a.png)
 
A doua secvență verifică existența unei remize în care un jucător nu are mutări posibile iar regele nu se află în șah (pat):
 ![image](https://user-images.githubusercontent.com/77692523/223124506-4531df64-7060-4eff-9da0-bd8a5ce86aef.png)
 
A treia secvență a funcției verifică dacă există remiză prin material insuficient:
 ![image](https://user-images.githubusercontent.com/77692523/223124534-102f1709-2355-4770-a4f4-1cba15b88738.png)

Funcția SfârșitJoc din clasa ChessGameController apelează funcția LaJocÎncheiat având ca parametru jucătorul câștigător în cazul unui câștig, sau remiză în cazul unei remize: 
 ![image](https://user-images.githubusercontent.com/77692523/223124593-a95237d7-2f64-4b31-88fd-14d53576176d.png)
 
Funcția LaJocÎncheiat din clasa ChessUIManager setează rezultatul jocului și deschide un meniu care arată rezultatul jocului:
 ![image](https://user-images.githubusercontent.com/77692523/223124630-c247daf4-4136-4b6a-8465-aee8e9b5df1f.png)
 
Exemplu – meniul la câștigarea jucătorului negru:
 ![image](https://user-images.githubusercontent.com/77692523/223124666-d5bcd47c-7ffb-4b5a-81fa-485173079f28.png)
 
Meniul mai conține două butoane. Unul de RESTART care începe o nouă partidă și unul de BACK care revine la meniul principal al aplicației.




Butonul RESTART are o acțiune onClick care apelează funcția RestartJoc din clasa ChessGameController. Aceasta distruge piesele din jocul anterior și creează unele noi după layout-ul inițial:
 ![image](https://user-images.githubusercontent.com/77692523/223124708-28c11d55-3e26-4062-8391-797a3160b0e8.png)
 
Butonul BACK are o acțiune onClick care apelează funcția ExitUI din clasa UIMenu care revine la meniul principal la aplicației:
 ![image](https://user-images.githubusercontent.com/77692523/223124731-bce68657-2e78-4ca6-bdc6-0dee60a61af8.png)
 
În timpul jocului, în partea dreapta jos a ecranului se află două butoane, NEW GAME și MAIN MENU. Butonul NEW GAME are o acțiune onClick care apelează funcția RestartJoc din clasa ChessGameController. În cazul în care jucătorul activ este cel negru la apăsarea butonului, camera își schimbă perspectiva la perspectiva jucătorului alb. Butonul MAIN MENU are o acțiune onClick care apelează funcția ExitUI din clasa UIMenu care revine la meniul principal la aplicației.
