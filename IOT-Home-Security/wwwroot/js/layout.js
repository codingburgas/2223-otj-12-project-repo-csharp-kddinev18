let homeContent = {
    content: [
        {
            title: "HomeTitle",
            bulets: ["Car Engine", "Milka", "Notebooks", "Kalai"],
        },
        {
            title: "HomeTitle2",
            bulets: ["Car Engine", "Milka", "Notebooks", "Kalai"],
        },
    ],
    imagePath: `"../wwwroot/images/img.png"`,
};

let extendedMenu = document.querySelector(".exteneded-menu");
function homeButtonHover() {
    extendedMenu.style.display = "flex";

    let image = document.createElement("img");
    image.setAttribute("src", homeContent.imgPath);
    image.classList.add("image");

    homeContent.content.forEach(function (item) {
        let linkRows = document.createElement("div");
        linkRows.classList.add("link-rows");

        let title = document.createElement("h3");
        title.classList.add("title");
        title.textContent = item.title;

        let line = document.createElement("hr");
        line.classList.add("line");

        let unorderedList = document.createElement("ul");

        item.bulets.forEach(function (item) {
            let listElement = document.createElement("li");
            listElement.textContent = item;

            unorderedList.appendChild(listElement);
        });

        linkRows.appendChild(title);
        linkRows.appendChild(line);
        linkRows.appendChild(unorderedList);

        extendedMenu.appendChild(linkRows);
    });

    extendedMenu.appendChild(image);
}

function removeDropdown() {
    extendedMenu.replaceChildren();
    extendedMenu.style.display = "none";
}
