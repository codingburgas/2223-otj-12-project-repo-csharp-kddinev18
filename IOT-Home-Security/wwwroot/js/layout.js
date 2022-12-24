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
};

let aboutContent = {
    content: [
        {
            title: "AboutTitle",
            bulets: ["Car Engine", "Milka", "Notebooks", "Kalai"],
        },
        {
            title: "AboutTitle2",
            bulets: ["Car Engine", "Milka", "Notebooks", "Kalai"],
        },
    ],
};

let portContent = {
    content: [
        {
            title: "PortTitle",
            bulets: ["Car Engine", "Milka", "Notebooks", "Kalai"],
        },
        {
            title: "PortTitle2",
            bulets: ["Car Engine", "Milka", "Notebooks", "Kalai"],
        },
    ],
};

let testContent = {
    content: [
        {
            title: "TestTitle",
            bulets: ["Car Engine", "Milka", "Notebooks", "Kalai"],
        },
        {
            title: "TestTitle2",
            bulets: ["Car Engine", "Milka", "Notebooks", "Kalai"],
        },
    ],
};

let extendedMenu = document.querySelector(".exteneded-menu");
function homeButtonHover() {
    extendedMenu.style.display = "flex";

    homeContent.content.forEach(displayExtendedMenu);
}

function aboutButtonHover() {
    extendedMenu.style.display = "flex";

    aboutContent.content.forEach(displayExtendedMenu);
}

function portButtonHover() {
    extendedMenu.style.display = "flex";

    portContent.content.forEach(displayExtendedMenu);
}

function testButtonHover() {
    extendedMenu.style.display = "flex";

    testContent.content.forEach(displayExtendedMenu);
}

function displayExtendedMenu (item) {
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

    extendedMenu.prepend(linkRows);
}

function removeDropdown() {
    while (extendedMenu.childNodes.length > 2) {
        console.log(extendedMenu.childNodes.length);
        extendedMenu.removeChild(extendedMenu.firstChild);
    }
    extendedMenu.style.display = "none";
}
