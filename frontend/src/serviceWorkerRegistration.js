export function register() {
  if ("serviceWorker" in navigator && process.env.NODE_ENV === "production") {
    window.addEventListener("load", () => {
      const swUrl = `${process.env.PUBLIC_URL}/service-worker.js`;

      navigator.serviceWorker
        .register(swUrl)
        .then((reg) => {
          console.log("ServiceWorker registered successfully: ", reg.scope);
        })
        .catch((error) => {
          console.error("ServiceWorker registration failed: ", error);
        });
    });
  } else if ("serviceWorker" in navigator && process.env.NODE_ENV === "development") {
    // Also register in dev mode to allow local testing if desired
    window.addEventListener("load", () => {
      const swUrl = `${process.env.PUBLIC_URL}/service-worker.js`;
      navigator.serviceWorker
        .register(swUrl)
        .then((reg) => {
          console.log("ServiceWorker registered in Dev Mode: ", reg.scope);
        })
        .catch((error) => {
          console.error("ServiceWorker registration failed: ", error);
        });
    });
  }
}

export function unregister() {
  if ("serviceWorker" in navigator) {
    navigator.serviceWorker.ready
      .then((registration) => {
        registration.unregister();
      })
      .catch((error) => {
        console.error(error.message);
      });
  }
}
