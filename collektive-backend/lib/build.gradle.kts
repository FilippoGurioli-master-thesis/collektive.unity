plugins {
    kotlin("multiplatform") version "2.2.21"
    id("it.unibo.collektive.collektive-plugin") version "27.4.0"
    id("com.squareup.wire") version "5.1.0"
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
    linuxX64("native") {
        binaries {
            sharedLib {
                baseName = "collektive-backend"
            }
        }
    }

    sourceSets {
        val commonMain by getting {
            dependencies {
                implementation("it.unibo.collektive:collektive-dsl:27.4.0")
                implementation("it.unibo.collektive:collektive-stdlib:27.4.0")
                implementation("com.squareup.wire:wire-runtime:5.5.0")
            }
        }
        val commonTest by getting {
            dependencies {
                implementation(kotlin("test"))
            }
        }
        val nativeMain by getting
        val nativeTest by getting
    }
}
