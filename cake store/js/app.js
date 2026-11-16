const API_URL = "https://localhost:7077/api/ui/About/GetAll";

async function getAbout() {

    try {

        const response = await fetch(API_URL);

        if (!response.ok) {
            throw new Error("HTTP Error: " + response.status);
        }

        const data = await response.json();

        console.log("API-dən gələn məlumat:", data);

        if (!data || data.length === 0) {
            console.log("About məlumatı yoxdur.");
            return;
        }

        const about = data[0];

        document.getElementById("about-title").textContent =
            about.title;

        document.getElementById("about-heading").textContent =
            about.heading;

        document.getElementById("about-description").textContent =
            about.description;

    }
    catch (error) {

        console.error("About API Error:", error);

        document.getElementById("about-title").textContent =
            "About Cake Shop";

        document.getElementById("about-heading").textContent =
            "Məlumat yüklənmədi";

        document.getElementById("about-description").textContent =
            "Serverə qoşulmaq mümkün olmadı.";

    }
}

getAbout();
