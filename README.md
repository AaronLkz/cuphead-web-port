# Cuphead - Isla 1 (Web Port) 🍵

Port experimental para navegador de **Cuphead: The Delicious Last Course (Isla 1)** utilizando **Unity WebGL**, **WebAssembly** y **GameCI**.

---

## ⚡ ¿Cómo compilar con GitHub Actions?

Este repositorio cuenta con compilación automatizada en la nube mediante **GameCI**:

### Paso 1: Activar Licencia Gratuita de Unity (Solo la primera vez)
1. Ve a la pestaña **Actions** de tu repositorio en GitHub.
2. Selecciona el workflow **Acquire Unity License (.alf)** y haz clic en **Run workflow**.
3. Al terminar (1-2 minutos), descarga el archivo artefacto `UnityActivationFile.alf`.
4. Ve a [license.unity.com](https://license.unity.com), sube ese archivo y selecciona la licencia gratuita **Unity Personal**.
5. Descarga el archivo de licencia `.ulf`.
6. En tu repositorio de GitHub, ve a **Settings > Secrets and variables > Actions** y crea un nuevo secreto:
   * **Nombre:** `UNITY_LICENSE`
   * **Valor:** Pega el contenido completo del archivo `.ulf` (ábrelo con un editor de texto).

### Paso 2: Ejecutar la Compilación WebGL
1. En la pestaña **Actions**, selecciona **Build Cuphead WebGL (Isla 1)**.
2. Haz clic en **Run workflow**.
3. Los servidores de GitHub (con 16 GB de RAM) compilarán el proyecto a WebGL.
4. Al finalizar, descarga el artefacto **`Cuphead-Isla1-WebGL.zip`** que contendrá el juego 100% listo para correr en cualquier navegador.

---

## 🎮 Contenido de la Isla 1 incluido
* Menú de inicio y selección de partida
* Casa de Elder Kettle e Introducción
* Tutorial de saltos y parry
* Mapa interactivo de la Isla 1
* **Los 5 Jefes:**
  * The Root Pack (Botánicos)
  * Goopy Le Grande (La baba azul)
  * Ribby & Croaks (Ranas)
  * Hilda Berg (Avioneta)
  * Cagney Carnation (Flor)
* Niveles Run 'n Gun (Forest Follies y Treetop Trouble)
* Tienda de Porkrind y Mausoleo I
