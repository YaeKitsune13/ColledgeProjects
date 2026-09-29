import { RESOURCES } from "./config.js";
import { api } from "./api.js";

const state = { resource: "institutions", editing: null }; // editing = строка или null
const $ = (id) => document.getElementById(id);

// ---------- helpers ----------

function el(tag, props = {}, ...children) {
  const node = Object.assign(document.createElement(tag), props);
  node.append(...children);
  return node;
}

function showError(message = "") {
  $("error").textContent = message;
  $("error").hidden = !message;
}

async function run(action) {
  try {
    showError();
    await action();
  } catch (e) {
    showError(e.message);
  }
}

// ---------- tabs ----------

function renderTabs() {
  $("tabs").replaceChildren(
    ...Object.entries(RESOURCES).map(([key, { title }]) =>
      el("button", {
        className: "tab" + (key === state.resource ? " active" : ""),
        textContent: title,
        onclick: () => {
          state.resource = key;
          state.editing = null;
          refresh();
        },
      }),
    ),
  );
}

// ---------- table ----------

function renderTable(rows) {
  const { title, fields } = RESOURCES[state.resource];
  $("list-title").textContent = title;

  if (!rows.length) {
    $("list").replaceChildren(
      el("div", { className: "empty", textContent: "Записей пока нет" }),
    );
    return;
  }

  const head = el(
    "tr",
    {},
    el("th", { textContent: "ID" }),
    ...fields.map(([, label]) => el("th", { textContent: label })),
    el("th"),
  );

  const body = rows.map((row) =>
    el(
      "tr",
      {},
      el("td", { textContent: row.id }),
      ...fields.map(([name]) => el("td", { textContent: row[name] ?? "" })),
      el(
        "td",
        {},
        el(
          "div",
          { className: "actions" },
          el("button", {
            className: "btn",
            textContent: "Изменить",
            onclick: () => startEdit(row),
          }),
          el("button", {
            className: "btn danger",
            textContent: "Удалить",
            onclick: () => remove(row),
          }),
        ),
      ),
    ),
  );

  $("list").replaceChildren(
    el("table", {}, el("thead", {}, head), el("tbody", {}, ...body)),
  );
}

// ---------- form ----------

function renderForm() {
  const { fields } = RESOURCES[state.resource];
  const row = state.editing;
  $("form-title").textContent = row
    ? `Изменить запись #${row.id}`
    : "Добавить запись";

  const inputs = fields.map(([name, label, type, def]) =>
    el(
      "label",
      { className: "field" },
      el("span", { textContent: label }),
      el("input", { name, type, value: row ? (row[name] ?? "") : (def ?? "") }),
    ),
  );

  const buttons = [
    el("button", {
      className: "btn primary",
      textContent: row ? "Сохранить" : "Создать",
    }),
  ];
  if (row) {
    buttons.push(
      el("button", {
        type: "button",
        className: "btn",
        textContent: "Отмена",
        onclick: () => {
          state.editing = null;
          renderForm();
        },
      }),
    );
  }
  $("form").replaceChildren(...inputs, ...buttons);
}

function readForm(form) {
  const data = {};
  for (const [name, , type] of RESOURCES[state.resource].fields) {
    const value = form.elements[name].value;
    data[name] =
      type === "number" ? (value === "" ? null : Number(value)) : value;
  }
  return data;
}

$("form").addEventListener("submit", (e) => {
  e.preventDefault();
  run(async () => {
    const data = readForm(e.target);
    if (state.editing) await api.update(state.resource, state.editing.id, data);
    else await api.create(state.resource, data);
    state.editing = null;
    await refresh();
  });
});

// ---------- actions ----------

function startEdit(row) {
  state.editing = row;
  renderForm();
  $("form").scrollIntoView({ behavior: "smooth" });
}

function remove(row) {
  if (!confirm(`Удалить запись #${row.id}?`)) return;
  run(async () => {
    await api.remove(state.resource, row.id);
    await refresh();
  });
}

async function refresh() {
  renderTabs();
  renderForm();
  await run(async () => renderTable(await api.list(state.resource)));
}

refresh();
