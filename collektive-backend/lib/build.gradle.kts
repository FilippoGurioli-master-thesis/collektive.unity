plugins {
    kotlin("multiplatform") version "2.3.21"
    id("it.unibo.collektive.collektive-plugin") version "28.2.3"
    id("com.squareup.wire") version "6.2.0"
}

repositories {
    mavenCentral()
}

wire {
    kotlin { }
    sourcePath {
        srcDir("src/commonMain/proto")
    }
}

kotlin {
    linuxX64("linux") {
        binaries { sharedLib { baseName = "collektive_backend" } }
    }
    mingwX64("windows") {
        binaries { sharedLib { baseName = "collektive_backend" } }
    }
    sourceSets {
        val commonMain by getting {
            dependencies {
                implementation("it.unibo.collektive:collektive-dsl:28.2.4")
                implementation("it.unibo.collektive:collektive-stdlib:28.2.4")
                implementation("com.squareup.wire:wire-runtime:6.2.0")
            }
        }
        val nativeMain by creating {
            dependsOn(commonMain)
        }
        val linuxMain by getting { dependsOn(nativeMain) }
        val windowsMain by getting { dependsOn(nativeMain) }

        val commonTest by getting {
            dependencies {
                implementation(kotlin("test"))
            }
        }
        val nativeTest by creating {
            dependsOn(commonTest)
        }
        val linuxTest by getting {
            dependsOn(nativeTest)
        }
        val windowsTest by getting {
            dependsOn(nativeTest)
        }
    }
}
