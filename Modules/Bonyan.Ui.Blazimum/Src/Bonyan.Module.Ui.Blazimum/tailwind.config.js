/** @type {import('tailwindcss').Config} */

const withMT = require("@material-tailwind/html/utils/withMT")

module.exports = withMT({
  content: ['./**/*.{razor,html,css,scss}'],
  theme: {
    extend: {},
  },
  plugins: [],
});