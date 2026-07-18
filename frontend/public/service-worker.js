const CACHE_NAME = "agriledger-cache-v1";
const ASSETS_TO_CACHE = [
  "/",
  "/index.html",
  "/manifest.json",
  "/logo192.png",
  "/logo512.png"
];

// Install Event
self.addEventListener("install", (e) => {
  e.waitUntil(
    caches.open(CACHE_NAME).then((cache) => {
      console.log("Caching app shell & assets...");
      return cache.addAll(ASSETS_TO_CACHE);
    })
  );
  self.skipWaiting();
});

// Activate Event
self.addEventListener("activate", (e) => {
  e.waitUntil(
    caches.keys().then((keys) => {
      return Promise.all(
        keys.map((key) => {
          if (key !== CACHE_NAME) {
            console.log("Removing old cache...", key);
            return caches.delete(key);
          }
        })
      );
    })
  );
  self.clients.claim();
});

// Fetch Event - Cache First for static assets, Network First for APIs
self.addEventListener("fetch", (e) => {
  const url = new URL(e.request.url);

  // Avoid caching API requests or non-GET requests
  if (url.pathname.startsWith("/api") || e.request.method !== "GET") {
    e.respondWith(
      (async () => {
        try {
          return await fetch(e.request);
        } catch (err) {
          console.warn("API/Non-GET fetch failed:", err);
          if (e.request.method === "GET") {
            const cached = await caches.match(e.request);
            if (cached) return cached;
          }
          
          return new Response(
            JSON.stringify({ error: "Network error occurred" }),
            { 
              status: 503, 
              headers: { "Content-Type": "application/json" } 
            }
          );
        }
      })()
    );
    return;
  }

  e.respondWith(
    (async () => {
      const cachedResponse = await caches.match(e.request);
      
      if (cachedResponse) {
        // Fetch in background to update cache (stale-while-revalidate)
        fetch(e.request)
          .then(async (networkResponse) => {
            if (networkResponse.status === 200) {
              const cache = await caches.open(CACHE_NAME);
              await cache.put(e.request, networkResponse);
            }
          })
          .catch((err) => console.log("Background fetch failed (offline)", err));
        
        return cachedResponse;
      }

      try {
        const networkResponse = await fetch(e.request);
        
        if (networkResponse && networkResponse.status === 200 && networkResponse.type === "basic") {
          const responseToCache = networkResponse.clone();
          const cache = await caches.open(CACHE_NAME);
          await cache.put(e.request, responseToCache);
        }
        
        return networkResponse;
      } catch (err) {
        console.warn("Fetch failed, serving cached fallback if available:", err);
        
        // If it's a navigation request (routing), fallback to /index.html
        if (e.request.mode === "navigate") {
          const fallback = await caches.match("/index.html");
          if (fallback) return fallback;
        }
        
        const fallbackAsset = await caches.match(e.request);
        if (fallbackAsset) return fallbackAsset;

        return new Response("Network error occurred", { 
          status: 408, 
          statusText: "Request Timeout" 
        });
      }
    })()
  );
});
