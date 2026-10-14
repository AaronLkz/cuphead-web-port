# Cuphead - Isla 1 (Web Port para GitHub Pages) 🍵

Port de **Cuphead: The Delicious Last Course (Isla 1)** adaptado para ejecutarse directamente en **GitHub Pages** mediante **Unity WebGL + WebAssembly**.

---

## 🚀 Cómo publicarlo y jugar directamente en GitHub Pages:

### 1. Activar GitHub Pages en el repositorio:
1. En tu repositorio de GitHub, ve a **Settings** > **Pages** (en el menú lateral izquierdo).
2. En la sección **Build and deployment > Source**, selecciona:
   👉 **GitHub Actions** (en lugar de "Deploy from a branch").

### 2. Activar la Licencia Gratuita de Unity (Solo la primera vez):
1. En la pestaña **Actions**, selecciona **Acquire Unity License (.alf)** y dale a **Run workflow**.
2. Al terminar (1 minuto), descarga el artefacto `UnityActivationFile.alf`.
3. Entra a [license.unity.com](https://license.unity.com), sube ese archivo y selecciona la licencia gratuita **Unity Personal**.
4. Descarga el archivo `.ulf` resultante.
5. En GitHub, ve a **Settings > Secrets and variables > Actions**, crea un secreto llamado `UNITY_LICENSE` y pega el texto del archivo `.ulf`.

### 3. ¡A Jugar!
1. Cada vez que hagas un push o ejecutes el workflow **Build and Deploy Cuphead WebGL to GitHub Pages**, los servidores de GitHub compilarán el juego y lo publicarán en la web automáticamente.
2. Tu enlace para jugar será:
   👉 **`https://<tu-usuario>.github.io/<nombre-del-repo>/`**

---

## 🎮 Contenido de la Isla 1
* Menú de inicio y selector de partidas persistente (guardado en navegador con IndexedDB).
* Casa de Elder Kettle e Introducción.
* Tutorial de saltos y parry.
* Mapa interactivo de la Isla 1.
* **Los 5 Jefes:**
  * *The Root Pack* (Botánicos)
  * *Goopy Le Grande* (La baba azul)
  * *Ribby & Croaks* (Ranas)
  * *Hilda Berg* (Avioneta)
  * *Cagney Carnation* (Flor)
* Niveles Run 'n Gun (*Forest Follies* y *Treetop Trouble*).
* Tienda de Porkrind y Mausoleo I.
