/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Views/**/*.cshtml",
    "./wwwroot/**/*.html",
    "./node_modules/flowbite/**/*.js"
  ],
  theme: {
    extend: {
      "colors": {
        "primary": "#3525cd",
        "on-surface": "#191c1d",
        "surface-container-lowest": "#ffffff",
        "on-primary": "#ffffff",
        "surface-container": "#edeeef",
        "inverse-surface": "#2e3132",
        "surface-dim": "#d9dadb",
        "tertiary-fixed": "#ffdbcc",
        "on-tertiary-fixed-variant": "#7b2f00",
        "on-secondary-fixed": "#140f54",
        "secondary": "#58579b",
        "on-tertiary-fixed": "#351000",
        "background": "#f8f9fa",
        "on-surface-variant": "#464555",
        "on-error": "#ffffff",
        "tertiary-fixed-dim": "#ffb695",
        "on-error-container": "#93000a",
        "outline": "#777587",
        "secondary-fixed-dim": "#c3c0ff",
        "inverse-on-surface": "#f0f1f2",
        "secondary-container": "#b6b4ff",
        "on-secondary": "#ffffff",
        "error": "#ba1a1a",
        "inverse-primary": "#c3c0ff",
        "on-secondary-fixed-variant": "#413f82",
        "tertiary-container": "#a44100",
        "on-tertiary-container": "#ffd2be",
        "on-secondary-container": "#454386",
        "on-primary-fixed": "#0f0069",
        "on-primary-container": "#dad7ff",
        "tertiary": "#7e3000",
        "primary-fixed-dim": "#c3c0ff",
        "outline-variant": "#c7c4d8",
        "surface-tint": "#4d44e3",
        "surface-container-highest": "#e1e3e4",
        "surface-bright": "#f8f9fa",
        "on-primary-fixed-variant": "#3323cc",
        "surface": "#f8f9fa",
        "primary-fixed": "#e2dfff",
        "primary-container": "#4f46e5",
        "surface-container-high": "#e7e8e9",
        "surface-container-low": "#f3f4f5",
        "surface-variant": "#e1e3e4",
        "error-container": "#ffdad6",
        "secondary-fixed": "#e2dfff",
        "on-tertiary": "#ffffff",
        "on-background": "#191c1d"
      },
      "borderRadius": {
        "DEFAULT": "0.125rem",
        "lg": "0.25rem",
        "xl": "0.5rem",
        "full": "0.75rem"
      },
      "fontFamily": {
        "headline": ["Inter"],
        "body": ["Inter"],
        "label": ["Inter"]
      }
    },
  },
  plugins: [
    require('flowbite/plugin')
  ],
}
