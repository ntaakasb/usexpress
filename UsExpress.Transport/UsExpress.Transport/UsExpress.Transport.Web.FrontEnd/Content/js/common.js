$(window).scroll(function() {
  if($(window).scrollTop() >= 200)
  {    
    $('#to_top').fadeIn();
  }
  else
  {   
    $('#to_top').fadeOut();
  }
});
var App = function() {
    var handleSidebarMenu = function() {       
        $(document).ready(function($) {  
            var $sidebar_trigger = $('.navbar-toggle'),
                $sidebar_icon = $('.toggle-icon'),
                $sidebar_content_overlay = $('.mask-content');
                
            
            $sidebar_trigger.on('click', function(event) {
                $sidebar_icon.toggleClass('is-clicked');
                $('.menu-box').toggleClass('is-open');
                $('#page_usexpress').toggleClass('hidden');
				$('#topbar').toggleClass('hidden');
                $sidebar_content_overlay.toggleClass('mask-is-open').one('webkitTransitionEnd otransitionend oTransitionEnd msTransitionEnd transitionend');
            });
            
            // close lateral menu clicking outside the menu itself
            $sidebar_content_overlay.on('click', function(event){
                if( !$(event.target).is('.navbar-toggle') ) {
                    $sidebar_icon.removeClass('is-clicked');
                    $sidebar_content_overlay.removeClass('mask-is-open');
                    $('.menu-box').removeClass('is-open');
                    $('#page_usexpress').removeClass('hidden');
					$('#topbar').removeClass('hidden');
                }
            });
        });
    }    
    return {
        init: function() {
            handleSidebarMenu();     
        }
    }
}();

$(document).ready(function() {
	$('nav#menu').mmenu();
	$('.search_icon').on('click', function(event) {
		$('.box_search_topbar').addClass('show_search');
	});
	$('.cancel_search').on('click', function(event) {
		$('.box_search_topbar').removeClass('show_search');
	});
    App.init();

	
    $('.order_silde .owl-carousel').owlCarousel({
		items:6,
		loop:false,
		margin:35,
		autoplay:false,		
		autoplayTimeout:7000,
		nav:true,
		smartSpeed:650,
		responsive:{
			0:{
				items:2
			},
			590:{
				items:3,
				margin:20
			},
			1000:{
				items:5
			},			
			1280:{
				items:6
			}
		}		
    });

    $(".notification-container .tab a").click(function (event) {
        $(".notification-container .tab a").removeClass("active")
        if(!$(this).hasClass("active")){
            $(this).addClass("active");
        }
        else{
            $(this).removeClass("active");
        }
        event.preventDefault();
        var tab = $(this).attr("href");
        $(".tab-content >div").not(tab).css("display", "none");
        $(tab).fadeIn();		
    });		
	
});




