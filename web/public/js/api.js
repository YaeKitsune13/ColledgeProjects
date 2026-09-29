import { API_BASE } from "./config.js";

async function request(method, path, body) {
  const response = await fetch(API_BASE + path, {
    method,
    headers: body ? { "Content-Type": "application/json" } : {},
    body: body ? JSON.stringify(body) : undefined,
  });

  if (!response.ok) {
    const text = await response.text();
    throw new Error(
      `${response.status} ${response.statusText}\n${text.slice(0, 500)}`,
    );
  }
  return response.status === 204 ? null : response.json();
}

export const api = {
  list: (resource) => request("GET", `/${resource}`),
  create: (resource, data) => request("POST", `/${resource}`, data),
  update: (resource, id, data) => request("PUT", `/${resource}/${id}`, data),
  remove: (resource, id) => request("DELETE", `/${resource}/${id}`),
};
