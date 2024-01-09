if(NOT TARGET game-activity::game-activity)
add_library(game-activity::game-activity STATIC IMPORTED)
set_target_properties(game-activity::game-activity PROPERTIES
    IMPORTED_LOCATION "C:/Users/enric/.gradle/caches/transforms-3/c6da488043b8f3167369588fb03ded40/transformed/jetified-games-activity-2.0.2/prefab/modules/game-activity/libs/android.armeabi-v7a/libgame-activity.a"
    INTERFACE_INCLUDE_DIRECTORIES "C:/Users/enric/.gradle/caches/transforms-3/c6da488043b8f3167369588fb03ded40/transformed/jetified-games-activity-2.0.2/prefab/modules/game-activity/include"
    INTERFACE_LINK_LIBRARIES ""
)
endif()

if(NOT TARGET game-activity::game-activity_static)
add_library(game-activity::game-activity_static STATIC IMPORTED)
set_target_properties(game-activity::game-activity_static PROPERTIES
    IMPORTED_LOCATION "C:/Users/enric/.gradle/caches/transforms-3/c6da488043b8f3167369588fb03ded40/transformed/jetified-games-activity-2.0.2/prefab/modules/game-activity_static/libs/android.armeabi-v7a/libgame-activity_static.a"
    INTERFACE_INCLUDE_DIRECTORIES "C:/Users/enric/.gradle/caches/transforms-3/c6da488043b8f3167369588fb03ded40/transformed/jetified-games-activity-2.0.2/prefab/modules/game-activity_static/include"
    INTERFACE_LINK_LIBRARIES ""
)
endif()

