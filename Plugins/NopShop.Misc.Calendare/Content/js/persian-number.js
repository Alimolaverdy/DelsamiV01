function numbersToPersian (el) {
	
	if(!el.parentNode.classList.contains('not-persian-number')){
    ['۰', '۱', '۲', '۳', '۴', '۵', '۶', '۷', '۸', '۹'].forEach((num, index) => {
        el.textContent = el.textContent.replace(new RegExp(index, 'g'), num);
    });
	}
};

function traverseToLeaf (el) {
    if (el.childNodes.length === 0) return;

    el
        .childNodes
        .forEach(node => {
            if (node.nodeType === 3 ) {
				
                numbersToPersian(node);
           }
			else 
			{
             traverseToLeaf(node);
            }
        });
};


$(document).ready(function () {
traverseToLeaf(document.querySelector('body'));
});

$('body').on('change', 'body', function() {
traverseToLeaf(document.querySelector('body'));
});

$(document).ajaxComplete(function () {
 traverseToLeaf(document.querySelector('body'));
});
